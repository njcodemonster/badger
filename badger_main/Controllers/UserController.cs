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
using badgerApi.Models;
using Newtonsoft.Json;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserRepo _UserRepo;
        ILoggerFactory _loggerFactory;
        private INotesAndDocHelper _NotesAndDoc;
        private int note_type = 5;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public UserController(ILoggerFactory loggerFactory, INotesAndDocHelper NotesAndDoc, IConfiguration config, IUserRepo UserRepo)
        {
            _config = config;
            _UserRepo = UserRepo;
            _loggerFactory = loggerFactory;
            _NotesAndDoc = NotesAndDoc;
            
        }

        [HttpPost("Authenticate")]
        public async Task<Users> Authenticate([FromBody]   string value)
        {
            Users _user = new Users();
            try
            {
                LogiDetails logiDetails = JsonConvert.DeserializeObject<LogiDetails>(value);
                _user = await _UserRepo.AuthenticateUser(logiDetails);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in authenticating user for login attempt with message" + ex.Message);
            }
            return _user;
        }
    }
}