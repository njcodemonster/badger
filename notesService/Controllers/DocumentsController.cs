﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using notesService.Models;
using notesService.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace notesService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentsRepository _DocsRepo;
        ILoggerFactory _loggerFactory;

        public DocumentsController(IDocumentsRepository DocsRepo, ILoggerFactory loggerFactory)
        {
            _DocsRepo = DocsRepo;
            _loggerFactory = loggerFactory;
        }
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        // GET: api/Notes/Reff/5
        [HttpGet("Reff/{id}/{type/}{limit}")]
        public async Task<List<Documents>> GetByReff(int id, int type ,int limit)
        {
            List<Documents> ToReturn = new List<Documents>();
            try
            {
                ToReturn = await _DocsRepo.GetAllByReffAsync(id,type, limit);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return ToReturn;
        }

        // POST: api/Notes
        [HttpPost("create")]
        public async Task<String> PostAsync([FromBody] string value)
        {

            string NewInsertionID = "0";
            try
            {
                Documents newDoc = JsonConvert.DeserializeObject<Documents>(value);
                NewInsertionID = await _DocsRepo.CreateAsync(newDoc);
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
