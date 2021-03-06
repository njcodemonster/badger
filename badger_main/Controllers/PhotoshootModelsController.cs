﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using GenericModals.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoshootModelsController : ControllerBase
    {
        private readonly IPhotoshootModelRepository _PhotoshootModelRepo;
        ILoggerFactory _loggerFactory;

        public PhotoshootModelsController(IPhotoshootModelRepository ModelRepo, ILoggerFactory loggerFactory)
        {
            _PhotoshootModelRepo = ModelRepo;
            _loggerFactory = loggerFactory;
        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get List of PhotoshootModels calling from "/PhotoshootModels/list"
        URL: api/PhotoshootModels/list
        Request: Get
        Input: /list
        output: List of PhotoshootModels
        */
        [HttpGet("list")]
        public async Task<ActionResult<List<PhotoshootModels>>> GetAsync()
        {
            List<PhotoshootModels> ToReturn = new List<PhotoshootModels>();
            try
            {
                return await _PhotoshootModelRepo.GetAll(0);
            }
            catch(Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message"+ex.Message);
                return ToReturn;
            }
           

        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get PhotoshootModels by id "api/PhotoshootModels/list/1"
        URL: api/PhotoshootModels/list/1
        Request: Get
        Input: int id of PhotoshootModels
        output: List of PhotoshootModels by id
        */
        [HttpGet("list/{id}")]
        public async Task<List<PhotoshootModels>> GetAsync(int id)
        {
            List<PhotoshootModels> ToReturn = new List<PhotoshootModels>();
            try
            {
                PhotoshootModels Res =  await _PhotoshootModelRepo.GetById(id);
                ToReturn.Add(Res);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data of all photoshoot models with message" + ex.Message);
               
            }
            return ToReturn;
        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Create new PhotoshootModels with events created in PhotoshootModels event and user event "api/PhotoshootModels/create"
        URL: api/PhotoshootModels/create
        Request: Post
        Input: FromBody, string 
        output: string of Last insert PhotoshootModels id
        */
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PhotoshootModels newModel = JsonConvert.DeserializeObject<PhotoshootModels>(value);
                NewInsertionID = await _PhotoshootModelRepo.Create(newModel);
            }
            catch(Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new photoshoot model with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update PhotoshootModels by id with events created in PhotoshootModels event and user event "api/PhotoshootModels/update/5"
        URL: api/PhotoshootModels/update/5
        Request: Put
        Input: int id, FormBody string  
        output: string
        */
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                PhotoshootModels ModelToUpdate = JsonConvert.DeserializeObject<PhotoshootModels>(value);
                ModelToUpdate.model_id = id;
                UpdateProcessOutput = await _PhotoshootModelRepo.Update(ModelToUpdate);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating photoshoot model with message" + ex.Message);
                UpdateResult = "Failed";
            }
            if (!UpdateProcessOutput)
            {
                UpdateResult = "Creation failed due to reason: No specific reson";
            }
            return UpdateResult;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update specific PhotoshootModels by id with events created in PhotoshootModels event and user event "api/PhotoshootModels/updatespecific/5"
        URL: api/PhotoshootModels/updatespecific/5
        Request: Put
        Input: int id, FormBody string  
        output: string
        */
        [HttpPut("updatespecific/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
           
            try
            {
                PhotoshootModels ModelToUpdate = JsonConvert.DeserializeObject<PhotoshootModels>(value);
                ModelToUpdate.model_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
                if(ModelToUpdate.model_name != null)
                {
                    ValuesToUpdate.Add("model_name", ModelToUpdate.model_name);
                }
                if (ModelToUpdate.model_height != null)
                {
                    ValuesToUpdate.Add("model_height", ModelToUpdate.model_height);
                }
                if (ModelToUpdate.model_ethnicity != null)
                {
                    ValuesToUpdate.Add("model_ethnicity", ModelToUpdate.model_ethnicity);
                }
                if (ModelToUpdate.model_hair != null)
                {
                    ValuesToUpdate.Add("model_hair", ModelToUpdate.model_hair);
                }
                if (ModelToUpdate.active_status != 0)
                {
                    ValuesToUpdate.Add("active_status", ModelToUpdate.active_status.ToString());
                }
                if (ModelToUpdate.updated_at != 0)
                {
                    ValuesToUpdate.Add("updated_at", ModelToUpdate.updated_at.ToString());
                }
                if (ModelToUpdate.updated_by != 0)
                {
                    ValuesToUpdate.Add("updated_by", ModelToUpdate.updated_by.ToString());
                }
                if (ModelToUpdate.created_by != 0)
                {
                    ValuesToUpdate.Add("created_by", ModelToUpdate.created_by.ToString());
                }
                if (ModelToUpdate.created_at != 0)
                {
                    ValuesToUpdate.Add("created_at", ModelToUpdate.created_at.ToString());
                }
                 
                await _PhotoshootModelRepo.UpdateSpecific(ValuesToUpdate, "model_id="+id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating photoshoot model with message" + ex.Message);
                UpdateResult = "Failed";
            }
            
            return UpdateResult;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
