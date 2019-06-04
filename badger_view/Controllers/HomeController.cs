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
           // Int32? test =  HttpContext.Session.GetInt32("isLogin");
           // if ( await _LoginHelper.CheckLogin())
           // {
                return View();
           // }
           // else
          //  {
          //      return View("Login");
           // }
        }
        [HttpPost("Dologin")]
        public async Task<IActionResult> DoLogin()
        {

            if (await _LoginHelper.DoLogin("x","y"))
            {
                return View("index");
            }
            else
            {
                return View("Login");
            }
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
