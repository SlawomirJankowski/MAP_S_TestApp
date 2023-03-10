using MAP_S_TestApp.Models;
using MAP_S_TestApp.Models.Domains;
using MAP_S_TestApp.Models.ViewModels;
using MAP_S_TestApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MAP_S_TestApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly ApplicationUserService _applicationUserService;
        public HomeController(ApplicationUserService applicationUserService)
        {
            _applicationUserService = applicationUserService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.UserName = _applicationUserService.GetLoggedUserName(User); 
            var vm = new AllApplicationUsersViewModel
            {
                ApplicationUsers = await _applicationUserService.GetAllUsersAsync(),
            };
            return View(vm);
        }

        
        public async Task<IActionResult> EditApplicationUser(int applicationUserId)
        {
            ViewBag.UserName = _applicationUserService.GetLoggedUserName(User);
            var applicationUser = await _applicationUserService.GetUserByIdAsync(applicationUserId);
            
            return View(applicationUser);
        }

        
        [HttpPost]
        public async Task<IActionResult> EditApplicationUser(ApplicationUser applicationUserModel)
        {
            ViewBag.UserName = _applicationUserService.GetLoggedUserName(User);
            var editedApplicationUser = await _applicationUserService.GetUserByIdAsync(applicationUserModel.Id);

            if (!ModelState.IsValid)
                return View("EditApplicationUser", applicationUserModel);

            if (_applicationUserService.EmailAlreadyExists(applicationUserModel.Email) && editedApplicationUser.Email != applicationUserModel.Email)
            {
                ViewBag.Error = "Podany adres e-mail jest już używany.\nWprowadź inny adres e-mail.";
                return View();
            }
                    
            await _applicationUserService.UpdateApplicationUserAsync(applicationUserModel);

            return RedirectToAction("Index", "Home");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}