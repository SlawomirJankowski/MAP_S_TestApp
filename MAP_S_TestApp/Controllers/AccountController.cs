using MAP_S_TestApp.Data;
using MAP_S_TestApp.Helpers;
using MAP_S_TestApp.Models.Domains;
using MAP_S_TestApp.Models.ViewModels;
using MAP_S_TestApp.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MAP_S_TestApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationUserRepository _applicationUserRepository;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _applicationUserRepository = new ApplicationUserRepository(context);
        }
             

        public IActionResult Register() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserViewModel registerUserModel)
        {
            if (!ModelState.IsValid)
                return View("Register", registerUserModel);

            if (_applicationUserRepository.EmailAlreadyExists(registerUserModel.Email))
            {
                ViewBag.Error = "Podany adres e-mail jest już używany.\nWprowadź inny adres e-mail.";
                return View();
            }

            PasswordHelper.CreatePasswordHash(
                registerUserModel.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var applicationUser = new ApplicationUser
            {
                FirstName = registerUserModel.FirstName,
                LastName = registerUserModel.LastName,
                Email = registerUserModel.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = DateTime.UtcNow,
                VerificationToken = PasswordHelper.CreateVerificationToken()
            };

            await _applicationUserRepository.AddUserAsync(applicationUser);

            //TODO: sent confirmation email
            var link = Url.ActionLink("AccountConfirmation", "Account", new { applicationUserId = applicationUser.Id, verificationToken = applicationUser.VerificationToken }, Request.Scheme, Request.Host.ToString());

            var emailSender = new EmailHelper();
            await emailSender.SendActivationLink(link, applicationUser.FirstName, applicationUser.LastName, applicationUser.Email);
            
            return RedirectToAction("RegisterConfirmation");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserViewModel loginUserModel)
        {
            if (!ModelState.IsValid)
                return View("Login", loginUserModel);

            var applicationUser = await _applicationUserRepository.GetUserByEmailAsync(loginUserModel.Email);

            if (applicationUser == null)
            {
                ViewBag.Error = "Nieprawidłowy login - spróbuj ponownie.";
                return View();
            }

            if (!PasswordHelper.VerifyPasswordHash(loginUserModel.Password, applicationUser.PasswordHash, applicationUser.PasswordSalt))
            {
                ViewBag.Error = "Nieprawidłowe hasło - spróbuj ponownie.";
                return View();
            }

            if (!applicationUser.IsActivated)
            {
                ViewBag.Error = "Konto nie zostało aktywowane. Sprawdź wiadomość z linkiem aktywacyjnym lub zarejestruj się ponownie.";
                return View();
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(AuthenticateHelper.CreateClaimsIdentity(applicationUser)));

            return RedirectToAction("Index", "Home");
        }

        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        public async Task<IActionResult> AccountConfirmation(int applicationUserId, string verificationToken)
        {
            var applicationUser = await _applicationUserRepository.GetUserByIdAndTokenAsync(applicationUserId, verificationToken);
            if (applicationUser == null)
            {
                ViewBag.Error = "Link aktywacyjny jest niepoprawny. Zarejestruj się ponownie.";
                return View();
            }

            if (applicationUser.CreatedAt.AddHours(2) < DateTime.UtcNow)
            {
                ViewBag.Error = "Link aktywacyjny stracił ważność. Zarejestruj się ponownie.";
                return View();
            }

            await _applicationUserRepository.SetIsActivatedStatusTrueAsync(applicationUser.Id);

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
