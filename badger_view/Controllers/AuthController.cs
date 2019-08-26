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


        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: View Login page default
        URL: /Dologin
        Request: Get
        Input: Null
        output: page of login
        */
        [HttpGet("Dologin")]
        public async Task<IActionResult> DoLogin()
        {

            return View("Login");

        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Check function login data and by pass on page
        URL: /TryLogin
        Request: Get
        Input: Login data
        output: Redirect to page
        */
        [HttpPost("TryLogin")]
        public async Task<IActionResult> TryLogin(badger_view.Models.LogiDetails logiDetails)
        {
            if (await _LoginHelper.DoLogin(logiDetails))
            {
                return RedirectToAction("Index", "PurchaseOrders");
            }
            else
            {
                return RedirectToAction("Dologin", "Auth");
            }

        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
           string user_id = await _LoginHelper.GetLoginUserId();

            if (await _LoginHelper.Logout(user_id))
            {
                return RedirectToAction("Dologin", "Auth");
            }
            else
            {
                return RedirectToAction("Index", "PurchaseOrders");
            }
        }

    }
}