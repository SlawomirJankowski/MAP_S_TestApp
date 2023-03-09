using MAP_S_TestApp.Data;
using MAP_S_TestApp.Models;
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
        private ApplicationUserRepository _applicationUserRepository;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
            _applicationUserRepository = new ApplicationUserRepository(context);
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claims = this.User.Claims.ToList();
            ViewBag.UserName = $"{claims[1].Value} {claims[2].Value}";
            var vm = new AllApplicationUsersViewModel
            {
                ApplicationUsers = await _applicationUserRepository.GetAllUsersAsync(),
            };
            return View(vm);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}