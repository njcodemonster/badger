using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using badgerApi.Helper;
using GenericModals.Models;
using Newtonsoft.Json;
using GenericModals.Event;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserRepo _UserRepo;
        ILoggerFactory _loggerFactory;
        private IEventRepo _eventRepo;
        private INotesAndDocHelper _NotesAndDoc;
        private int note_type = 5;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();

        string user_login = "user_login";
        string user_logout = "user_logout";

        public UserController(ILoggerFactory loggerFactory, INotesAndDocHelper NotesAndDoc, IConfiguration config, IUserRepo UserRepo,IEventRepo eventRepo)
        {
            _eventRepo = eventRepo;
            _config = config;
            _UserRepo = UserRepo;
            _loggerFactory = loggerFactory;
            _NotesAndDoc = NotesAndDoc;
            
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Login form data send 
        URL: /User/Authenticate
        Request: Post
        Input:  FromBody string 
        output: dynamic object of users
        */
        [HttpPost("Authenticate")]
        public async Task<Users> Authenticate([FromBody]   string value)
        {
            Users _user = new Users();
            try
            {
                LogiDetails logiDetails = JsonConvert.DeserializeObject<LogiDetails>(value);
                _user = await _UserRepo.AuthenticateUser(logiDetails);

                var userEvent = new EventModel("user_events")
                {
                    EventName = user_login,
                    EntityId = _user.user_id,
                    RefrenceId = _user.user_id,
                    UserId = _user.user_id,
                    EventNoteId = _user.user_id
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in authenticating user for login attempt with message" + ex.Message);
            }
            return _user;
        }
        /*
      Developer: Sajid Khan
      Date: 7-31-19 
      Action: logout and sending user event
      URL: /User/LogoutAuthenticate
      Request: Post
      Input:  user_id
      output: string
      */
        [HttpPost("LogoutAuthenticate")]
        public async Task<String> LogoutAuthenticate([FromBody]   string value)
        {
            string result = "0";
            try
            {
                dynamic LogiDetails = JsonConvert.DeserializeObject<Object>(value);
                int user_id = LogiDetails.user_id;

                var userEvent = new EventModel("user_events")
                {
                    EventName = user_logout,
                    EntityId = user_id,
                    RefrenceId = user_id,
                    UserId =user_id,
                    EventNoteId = user_id
                };
                await _eventRepo.AddEventAsync(userEvent);

                result = "1";
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in authenticating user for login attempt with message" + ex.Message);
            }
            return result;
        }
    }
}