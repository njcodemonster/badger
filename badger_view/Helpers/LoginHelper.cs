using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace badger_view.Models
{
    [Table("users")]
    public partial class Users
    {
        [Key]
        public int user_id { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string full_name { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string designation { get; set; }
        public int access_level_id { get; set; }
        public double? last_login { get; set; }
        public double? last_session { get; set; }
        public int active_status { get; set; }
        public double created_at { get; set; }
        public double? updated_at { get; set; }
        public int created_by { get; set; }
        public int? updated_by { get; set; }
    }
}

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
            Boolean isLoedIn = false;
            if (UserEmail == "Testing@test.com" && Password == "Testing")
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("isLogin", 1);
                 isLoedIn = true;
            }
            else
            {
                 isLoedIn = false;
            }
          
            return isLoedIn;
        }
    }
}
