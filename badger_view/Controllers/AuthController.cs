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
    [ApiController]
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
            var claim = new List < Claim >{
                new Claim(ClaimTypes.NameIdentifier, "Ponka@Ponka.com"),
                new Claim(ClaimTypes.Name,"Ponka"),
            };
            var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            if (await _LoginHelper.DoLogin("x", "y"))
            {
                return View("index");
            }
            else
            {
                return View("Login");
            }
        }
    }
}