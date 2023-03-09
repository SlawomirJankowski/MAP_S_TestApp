using MAP_S_TestApp.Data;
using MAP_S_TestApp.Models;
using MAP_S_TestApp.Models.Domains;
using MAP_S_TestApp.Models.ViewModels;
using MAP_S_TestApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MAP_S_TestApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationUserRepository _applicationUserRepository;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
            _applicationUserRepository = new ApplicationUserRepository(context);
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            ViewBag.UserName = GetLoggedUserName();
            var vm = new AllApplicationUsersViewModel
            {
                ApplicationUsers = await _applicationUserRepository.GetAllUsersAsync(),
            };
            return View(vm);
        }

        public async Task<IActionResult> EditApplicationUser(int applicationUserId)
        {
            ViewBag.UserName = GetLoggedUserName();
            var applicationUser = await _applicationUserRepository.GetUserByIdAsync(applicationUserId);
            return View(applicationUser);
        }

        [HttpPost]
        public async Task<IActionResult> EditApplicationUser(ApplicationUser applicationUserModel)
        {
            ViewBag.UserName = GetLoggedUserName();
            if (!ModelState.IsValid)
                return View("Register", applicationUserModel);

            if (_applicationUserRepository.EmailAlreadyExists(applicationUserModel.Email))
            {
                ViewBag.Error = "Podany adres e-mail jest już używany.\nWprowadź inny adres e-mail.";
                return View();
            }
                      
            var updatedApplicationUser = new ApplicationUser
            {
                Id = applicationUserModel.Id,
                FirstName = applicationUserModel.FirstName,
                LastName = applicationUserModel.LastName,
                Email = applicationUserModel.Email,
            };

            await _applicationUserRepository.UpdateApplicationUserAsync(updatedApplicationUser);

            return RedirectToAction("Index", "Home");
        }

        private string GetLoggedUserName()
        {
            var claims = this.User.Claims.ToList();
            return $"{claims[1].Value} {claims[2].Value}";
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}