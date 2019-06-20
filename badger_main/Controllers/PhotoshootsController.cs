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
        private IPhotoshootRepository _PhotoshootRepo;
        ILoggerFactory _loggerFactory;
        private IEventRepo _eventRepo;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        string table_name = "product_events";

        string event_create_photoshoot = "Photoshoot created by user =%%userid%% with photoshoot id = %%pid%%"; ///event_create_photoshoot
        int event_photoshoot_created_id = 16;

        string event_photoshoot_started = "Photoshoot started by user =%%userid%% with product id = %%pid%%"; ///event_create_photoshoot
        int event_photoshoot_started_id = 17;

        string event_photoshoot_not_started = "Photoshoot not started by user =%%userid%% with product id = %%pid%%"; ///event_create_photoshoot
        int event_photoshoot_not_started_id = 18;

        string event_photoshoot_sent_to_editor = "Photoshoot sent to editor by user =%%userid%% with product id = %%pid%%"; ///event_create_photoshoot
        int event_photoshoot_sent_to_editor_id = 19;

        string user_event_create_photoshoot = "Photoshoot created with photoshoot id = %%pid%%"; 
        string user_event_photoshoot_started = "Photoshoot started product id = %%pid%%"; 
        string user_event_photoshoot_not_started = "Photoshoot not started with product id = %%pid%%"; 
        string user_event_photoshoot_sent_to_editor = "Photoshoot sent to editor with product id = %%pid%%";
        
        public PhotoshootsController(IPhotoshootRepository PhotoshootRepo, ILoggerFactory loggerFactory, IEventRepo eventRepo)
        {
            _eventRepo = eventRepo;
            _PhotoshootRepo = PhotoshootRepo;
            _loggerFactory = loggerFactory;
        }

        // GET: api/Photoshoots/list
        [HttpGet("list")]
        public async Task<ActionResult<List<Photoshoots>>> GetAsync()
        {
            List<Photoshoots> ToReturn = new List<Photoshoots>();
            try
            {
                return await _PhotoshootRepo.GetAll(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }
        }

        [HttpGet("inprogress")]
        public async Task<ActionResult<Object>> GetInprogress()
        {
            dynamic ToReturn = new object();
            try
            {
                return await _PhotoshootRepo.GetInprogressPhotoshoot(0);
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
            return await _PhotoshootRepo.Count();

        }
     
        [HttpGet("list/{id}")]
        public async Task<List<Photoshoots>> GetAsync(int id)
        {
            List<Photoshoots> ToReturn = new List<Photoshoots>();
            try
            {
                Photoshoots Res = await _PhotoshootRepo.GetById(id);
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
                vPageList = await _PhotoshootRepo.GetPhotoshootDetailsRep(limit);
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
        
        [HttpGet("GetPhotoshootsProducts/{photoshootId}")]
        public async Task<object> GetPhotoshootsProducts(int photoshootId)
        {
            dynamic vPageList = new object();
            try
            {
                vPageList = await _PhotoshootRepo.GetPhotoshootProducts(photoshootId);
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
                photoshootsAndModels  = await _PhotoshootRepo.GetAllPhotoshoots(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for PhotoshootsAndModels with message" + ex.Message);

            }

            return photoshootsAndModels;

        }

        [HttpGet("SendToEditorPhotoshoot")]
        public async Task<object> SendToEditorPhotoshoot()
        {
            dynamic SendToEditorProduct = new object();
            try
            {
                SendToEditorProduct = await _PhotoshootRepo.GetSendToEditorPhotoshoot(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for SendToEditor Product with message" + ex.Message);

            }

            return SendToEditorProduct;

        }


        // POST: api/photoshoots/create
        [HttpPost("create/{productId}")]
        public async Task<string> PostAsync([FromBody]   string value, int productId)
        {
            string NewInsertionID = "0";
            try
            {
                Photoshoots newPhotoshoots = JsonConvert.DeserializeObject<Photoshoots>(value);
                NewInsertionID = await _PhotoshootRepo.Create(newPhotoshoots);
                int userId = newPhotoshoots.created_by;
                event_create_photoshoot = event_create_photoshoot.Replace("%%userid%%", userId.ToString());
                event_create_photoshoot = event_create_photoshoot.Replace("%%pid%%", NewInsertionID.ToString());
                await _eventRepo.AddPhotoshootAsync(productId, event_photoshoot_created_id, Int32.Parse(NewInsertionID), event_create_photoshoot, userId, _common.GetTimeStemp(), table_name);
                 

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Photoshoots with message" + ex.Message);
            }
            return NewInsertionID;
        }


        [HttpPut("UpdatePhotoshootProductStatus/{productId}")]
        public async Task<string> UpdatePhotoshootProductStatus(int productId, [FromBody]   string value)
        {
            string UpdateResult = "Success";
            try
            {
                ProductPhotoshootStatusUpdate PhotoshootToUpdate = JsonConvert.DeserializeObject<ProductPhotoshootStatusUpdate>(value);
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
                ValuesToUpdate.Add("product_shoot_status_id", PhotoshootToUpdate.product_shoot_status_id.ToString());
                ValuesToUpdate.Add("updated_by", PhotoshootToUpdate.updated_by.ToString());
                ValuesToUpdate.Add("updated_at", PhotoshootToUpdate.updated_at.ToString());
                int userId = PhotoshootToUpdate.updated_by;
                await _PhotoshootRepo.UpdateSpecific(ValuesToUpdate, " product_id = " + productId);
                string PhotoshootStatus = PhotoshootToUpdate.product_shoot_status_id.ToString();

                if (PhotoshootStatus == "1")
                {
                    event_photoshoot_started = event_photoshoot_started.Replace("%%userid%%", userId.ToString());
                    event_photoshoot_started = event_photoshoot_started.Replace("%%pid%%", productId.ToString());
                    await _eventRepo.AddPhotoshootAsync(productId, event_photoshoot_started_id, productId, event_photoshoot_started, userId, _common.GetTimeStemp(), table_name);
                }
                else if (PhotoshootStatus == "2")
                {
                    event_photoshoot_sent_to_editor = event_photoshoot_sent_to_editor.Replace("%%userid%%", userId.ToString());
                    event_photoshoot_sent_to_editor = event_photoshoot_sent_to_editor.Replace("%%pid%%", productId.ToString());
                    await _eventRepo.AddPhotoshootAsync(productId, event_photoshoot_sent_to_editor_id, productId, event_photoshoot_sent_to_editor, userId, _common.GetTimeStemp(), table_name);
                }
                else if (PhotoshootStatus == "0")
                {
                    event_photoshoot_not_started = event_photoshoot_not_started.Replace("%%userid%%", userId.ToString());
                    event_photoshoot_not_started = event_photoshoot_not_started.Replace("%%pid%%", productId.ToString());
                    await _eventRepo.AddPhotoshootAsync(productId, event_photoshoot_not_started_id, productId, event_photoshoot_not_started, userId, _common.GetTimeStemp(), table_name);
                }

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in product Send To Editor with message" + ex.Message);
                UpdateResult = "Failed";
            }

            return UpdateResult;
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

                await _PhotoshootRepo.UpdateSpecific(ValuesToUpdate, "product_id IN (" + productId + ")");

                int userId = PhotoshootToUpdate.updated_by;
                int photoshootId = PhotoshootToUpdate.photoshoot_id;

                event_photoshoot_started = event_photoshoot_started.Replace("%%userid%%", userId.ToString());
                event_photoshoot_started = event_photoshoot_started.Replace("%%pid%%", productId.ToString());
                await _eventRepo.AddPhotoshootAsync(Int32.Parse(productId), event_photoshoot_started_id, photoshootId, event_photoshoot_started, userId, _common.GetTimeStemp(), table_name);

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