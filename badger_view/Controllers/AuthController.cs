using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using badger_view.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using badger_view.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace badger_view.Controllers
{
    [Route("[controller]")]
    
    public class AuthController : Controller
    {
        ILoginHelper _LoginHelper;
        public AuthController(ILoginHelper loginHelper)
        {
            _LoginHelper = loginHelper;
        }
        [HttpGet("Dologin")]
        public async Task<IActionResult> DoLogin()
        {

            return View("Login");

        }
        [HttpPost("TryLogin")]
        public async Task<IActionResult> TryLogin(badger_view.Models.LogiDetails logiDetails)
        {
            if (await _LoginHelper.DoLogin(logiDetails))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("/Login");
            }

        }
        
    }
}