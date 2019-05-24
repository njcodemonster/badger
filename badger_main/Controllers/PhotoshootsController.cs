using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using badgerApi.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;

namespace badgerApi.Controllers
{ 

    [Route("api/[controller]")]
    [ApiController]
    public class PhotoshootsController : ControllerBase
    {
        private readonly IPhotoshootsRepository _PhotoshootsRepo;
        ILoggerFactory _loggerFactory;

        public PhotoshootsController(IPhotoshootsRepository PhotoshootsRepo, ILoggerFactory loggerFactory)
        {

            _PhotoshootsRepo = PhotoshootsRepo;
            _loggerFactory = loggerFactory;
        }

        // GET: api/Photoshoots/list
        [HttpGet("list")]
        public async Task<ActionResult<List<Photoshoots>>> GetAsync()
        {
            List<Photoshoots> ToReturn = new List<Photoshoots>();
            try
            {
                return await _PhotoshootsRepo.GetAll(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }


        }
        // GET: api/Photoshoots/count
        [HttpGet("count")]
        public async Task<string> CountAsync()
        {
            return await _PhotoshootsRepo.Count();

        }
     
        [HttpGet("list/{id}")]
        public async Task<List<Photoshoots>> GetAsync(int id)
        {
            List<Photoshoots> ToReturn = new List<Photoshoots>();
            try
            {
                Photoshoots Res = await _PhotoshootsRepo.GetById(id);
                ToReturn.Add(Res);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return ToReturn;
        }

        // GET: api/Photoshoots/listpageview/10
        [HttpGet("listpageview/{limit}")]
        public async Task<object> listpageviewAsync(int limit)
        {
            dynamic vPageList = new object();
            try
            {
                vPageList = await _PhotoshootsRepo.GetPhotoshootsDetailsRep(limit);
                //string vPageCount = await _PhotoshootsRepo.Count();
                //vPageList.Count = vPageCount;
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return vPageList;

        }


        [HttpGet("photoshootsAndModels")]
        public async Task<object> photoshootsAndModels()
        {
            dynamic photoshootsAndModels = new object();
            try
            {
                photoshootsAndModels  = await _PhotoshootsRepo.GetAllPhotoshoots(0);
              //  var photoshootsModels = await _PhotoshootsRepo.GetAllPhotoshootsModels(0);
             //   photoshootsAndModels.photoshootsModels = photoshootsModels;

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for PhotoshootsAndModels with message" + ex.Message);

            }

            return photoshootsAndModels;

        }

        // POST: api/photoshoots/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                Photoshoots newPhotoshoots = JsonConvert.DeserializeObject<Photoshoots>(value);
                NewInsertionID = await _PhotoshootsRepo.Create(newPhotoshoots);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Photoshoots with message" + ex.Message);
            }
            return NewInsertionID;
        }
        [HttpPost("assignProductPhotoshoot/{productId}")]
        public async Task<string> assignProductPhotoshoot(string productId, [FromBody]   string value)
        {
            string UpdateResult = "Success";
            try
            {

                ProductPhotoshoots PhotoshootToUpdate = JsonConvert.DeserializeObject<ProductPhotoshoots>(value);
                
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
                ValuesToUpdate.Add("photoshoot_id", PhotoshootToUpdate.photoshoot_id.ToString());
                ValuesToUpdate.Add("product_shoot_status_id", PhotoshootToUpdate.product_shoot_status_id.ToString());
                ValuesToUpdate.Add("updated_by", PhotoshootToUpdate.updated_by.ToString());
                ValuesToUpdate.Add("updated_at", PhotoshootToUpdate.updated_at.ToString());

                await _PhotoshootsRepo.UpdateSpecific(ValuesToUpdate, "product_id IN (" + productId + ")");
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  item barcode with message" + ex.Message);
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