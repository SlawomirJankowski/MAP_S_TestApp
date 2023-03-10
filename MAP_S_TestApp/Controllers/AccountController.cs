using MAP_S_TestApp.Helpers;
using MAP_S_TestApp.Models.ViewModels;
using MAP_S_TestApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MAP_S_TestApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationUserService _applicationUserService;

        public AccountController(ApplicationUserService applicationUserService)
        {
            _applicationUserService = applicationUserService;
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

            if (_applicationUserService.EmailAlreadyExists(registerUserModel.Email))
            {
                ViewBag.Error = "Podany adres e-mail jest już używany.\nWprowadź inny adres e-mail.";
                return View();
            }

            await _applicationUserService.AddUserAsync(registerUserModel, this.HttpContext);
                       
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

            var applicationUser = await _applicationUserService.GetUserByEmailAsync(loginUserModel.Email);

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
            var applicationUser = await _applicationUserService.GetUserByIdAndTokenAsync(applicationUserId, verificationToken);
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

            await _applicationUserService.SetIsActivatedStatusTrueAsync(applicationUser.Id);

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
