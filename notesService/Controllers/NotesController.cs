using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GenericModals.Models;
using notesService.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace notesService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {

        private readonly INotesRepository _NotesRepo;
        ILoggerFactory _loggerFactory;

        public NotesController(INotesRepository NotesRepo, ILoggerFactory loggerFactory)
        {
            _NotesRepo = NotesRepo;
            _loggerFactory = loggerFactory;
        }
        // GET: api/Notes
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "Note" };
        }

        // GET: api/Notes/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get Notes list "api/Notes/Reff/5/5/1"
        URL: api/Notes/Reff/5/5/1
        Request: Get
        Input: int reffence id,int notetype, int Limit
        output: List of Notes
        */
        [HttpGet("Reff/{id}/{notetype}/{limit}")]
        public async Task<List<Notes>> GetByReff(int id,int notetype, int Limit)
        {
            List<Notes> ToReturn = new List<Notes>();
            
            try
            {
                ToReturn = await _NotesRepo.GetAllByReff(id, notetype, Limit);   
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);
            }
           
            return ToReturn;
        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get Notes list "api/Notes/Reff/5,2,44,1/5"
        URL: api/Notes/Reff/5,2,44,1/5
        Request: Get
        Input: string reffences ids,int notetype
        output: List of Notes
        */
        [HttpGet("Reff/{ids}/{notetype}")]
        public async Task<List<Notes>> GetAllNotesByReffs(string ids, int notetype)
        {
            List<Notes> ToReturn = new List<Notes>();
            try
            {
                ToReturn = await _NotesRepo.GetAllNotesByReffs(ids, notetype);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAllNotesByReffs with message" + ex.Message);
            }

            return ToReturn;
        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Create new Notes "api/Notes/create"
        URL: api/Notes/create
        Request: Post
        Input: [FromBody] string value
        output: string last insert note id
        */
        [HttpPost("create")]
        public async Task<String> PostAsync([FromBody] string value)
        {

            string NewInsertionID = "0";
            try
            {
                Notes newNote = JsonConvert.DeserializeObject<Notes>(value);
                NewInsertionID = await _NotesRepo.Create(newNote);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new vendor with message" + ex.Message);
            }
            return NewInsertionID;

        }

        // PUT: api/Notes/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
