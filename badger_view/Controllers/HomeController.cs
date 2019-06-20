using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badger_view.Models;
using badger_view.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace badger_view.Controllers
{
    public class HomeController : Controller
    {
        ILoginHelper _LoginHelper;
        public HomeController(ILoginHelper loginHelper)
        {
            _LoginHelper = loginHelper;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpPost("Dologin")]
        public async Task<IActionResult> DoLogin()
        {
            return View("Login");

        }
        [Authorize]
        public async Task<IActionResult> About()
        {
            ViewData["Message"] = "Your application description page.";
            
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Form()
        {
            return View();
        }
    }
}
