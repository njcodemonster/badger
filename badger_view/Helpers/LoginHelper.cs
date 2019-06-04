using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badger_view.Helpers
{
    public interface ILoginHelper
    {
       
        Task<Boolean> CheckLogin();
        Task<Boolean> DoLogin(String UserEmail, String Password);
    }
    public class LoginHelper : ILoginHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoginHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Boolean> CheckLogin()
        {
            Int32? isLogin = _httpContextAccessor.HttpContext.Session.GetInt32("isLogin");
            Boolean isLoedIn = false;
            if (isLogin == 1)
            {
                isLoedIn = true;
            }
            
            return  isLoedIn;
        }

        public async Task<bool> DoLogin(string UserEmail, string Password )
        {
            // HttpContext httpContext = new HttpContext();
            //HttpContext.Session.SetInt32("logedIn", 1);
            _httpContextAccessor.HttpContext.Session.SetInt32("isLogin", 1);
            Boolean isLoedIn = false;
            return isLoedIn;
        }
    }
}
