using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GenericModals.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using GenericModals;

namespace badger_view.Models_Backup
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
    public partial class LogiDetails
    {
        public String UserIdentity { get; set; }
        public String UserPass { get; set; }
    }

}

namespace badger_view.Helpers
{
   
    public interface ILoginHelper
    {
       
        Task<Boolean> CheckLogin();
        Task<Boolean> DoLogin(badger_view.Models_Backup.LogiDetails logiDetails);
        Task<string> GetLoginUserId();
        Task<string> GetLoginUserFirstName();
        Task<bool> Logout(string user_id);
    }
    public class LoginHelper : ILoginHelper
    {
        private BadgerApiHelper _BadgerApiHelper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoginHelper(IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }
        private void SetBadgerHelper()
        {
            if (_BadgerApiHelper == null)
            {
                _BadgerApiHelper = new BadgerApiHelper(_config);
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: validate user is login or not
        Request: Get 
        Input: 
        output: boolean
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get Login user id 
        Request: Get 
        Input: 
        output: string login id
        */
        public async Task<string> GetLoginUserId()
        {
            string id = "0";
            try
            {
                 id =  _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "PrimaryID").Value;
            }
            catch(Exception ex)
            {
                return "0";
            }
            return id;
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get Login user Firstname 
        Request: Get 
        Input: 
        output: string login Firstname
        */
        public async Task<string> GetLoginUserFirstName()
        {
            string id = "0";
            try
            {
                id = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "FirstName").Value;
            }
            catch (Exception ex)
            {
                return "0";
            }
            return id;
        }


        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Login Form data send to user validate record and save in cookie
        Request: Get 
        Input:  Login data email and password
        output: boolean
        */
        public async Task<bool> DoLogin(badger_view.Models_Backup.LogiDetails logiDetails)
        {
            SetBadgerHelper();
            Boolean isLoedIn = false;
            JObject LoginInfo = new JObject();
            LoginInfo.Add("UserIdentity",logiDetails.UserIdentity);
            LoginInfo.Add("UserPass", logiDetails.UserPass);
            String AuthenticatedUser = await _BadgerApiHelper.GenericPostAsyncString<String>(LoginInfo.ToString(Formatting.None), "/user/Authenticate");
            Users AuthUser = await _BadgerApiHelper.ForceConvert<Users>(AuthenticatedUser);

            if (AuthUser.user_id != 0)
            {
               
                var claim = new List < Claim >{
                        new Claim(ClaimTypes.NameIdentifier, AuthUser.email),
                        new Claim("FirstName", AuthUser.first_name),
                        new Claim(ClaimTypes.Name,AuthUser.full_name),
                        new Claim(ClaimTypes.Role,AuthUser.access_level_id.ToString()),
                        new Claim("PrimaryID",AuthUser.user_id.ToString()),
                };
                var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                isLoedIn = true;
            }
            else
            {
                 isLoedIn = false;
            }
          
            return isLoedIn;
        }

        public async Task<bool> Logout(string user_id)
        {
            SetBadgerHelper();
            JObject LoginInfo = new JObject();
            LoginInfo.Add("user_id", user_id);
            await _BadgerApiHelper.GenericPostAsyncString<String>(LoginInfo.ToString(Formatting.None), "/user/LogoutAuthenticate");

            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            return true;
        }
    }
}
