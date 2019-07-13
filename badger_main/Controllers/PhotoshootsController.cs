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
using System.Collections;
using badgerApi.Helper;

namespace badgerApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PhotoshootsController : ControllerBase
    {
        private IPhotoshootRepository _PhotoshootRepo;
        private IItemServiceHelper _ItemServiceHelper;

        ILoggerFactory _loggerFactory;
        private IEventRepo _eventRepo;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        string table_name = "product_events";
        string user_event_table_name = "user_events";

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

        public PhotoshootsController(IPhotoshootRepository PhotoshootRepo, ILoggerFactory loggerFactory, IEventRepo eventRepo, IItemServiceHelper ItemServiceHelper)
        {
            _ItemServiceHelper = ItemServiceHelper;
            _eventRepo = eventRepo;
            _PhotoshootRepo = PhotoshootRepo;
            _loggerFactory = loggerFactory;
        }

        /*
        Update Developer: Mohi
        Date: 7-3-19 
        Action: List all photoshoot products which are not started calling from { '/photoshoots/' }
        URL: /Photoshoots/list/
        Request: GET
        Input: int PhotoshootId
        output:  List of Photoshoots
        */
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

        /*
        Update Developer: Mohi
        Date: 7-3-19 
        Action: List all photoshoot products which are in-process calling from { '/photoshoots/shootInProgress' }
        URL: /Photoshoots/inprogress/
        Request: GET
        Input: Null
        output:   dynamic Object of Photoshoots 
        */
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

        /*
        Update Developer: Mohi
        Date: 7-3-19 
        Action: select photoshoot by ID 
        URL: /Photoshoots/list/1
        Request: GET
        Input: int PhotoshootId
        output: List of Photoshoots 
        */
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

        /*
        Update Developer: Mohi
        Date: 7-3-19 
        Action: List all photoshoot products with the given limit which are not started calling from { '/photoshoots/' }
        URL: /Photoshoots/listpageview/1/
        Request: GET
        Input: int limit
        output:  Object Photoshoot products 
        */
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

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: List all photoshoot product by photoshoot ID which are in-progress calling from { 'photoshoots/getPhotoshootInProgressProducts/' }
        URL: /Photoshoots/GetPhotoshootsProducts/1/
        Request: GET
        Input: int PhotoshootId
        output:  dynamic Object of Photoshoot products 
        */
        [HttpGet("GetPhotoshootsProducts/{photoshootId}")]
        public async Task<object> GetPhotoshootsProducts(int photoshootId)
        {
            dynamic vPageList = new object();
            try
            {
                vPageList = await _PhotoshootRepo.GetPhotoshootProducts(photoshootId);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetPhotoshootsProducts with message" + ex.Message);

            }

            return vPageList;

        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: List all photoshoots and models to add new photoshoot or add new product in already exists photoshoots calling from { 'photoshoots/getPhotoshootAndModels/' }
        URL: /Photoshoots/GetPhotoshootsAndModels/
        Request: GET
        Input: Null
        output:  dynamic Object of Photoshoots & Models
        */
        [HttpGet("GetPhotoshootsAndModels")]
        public async Task<object> photoshootsAndModels()
        {
            dynamic photoshootsAndModels = new object();
            try
            {
                photoshootsAndModels = await _PhotoshootRepo.GetAllPhotoshootsAndModels();
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for PhotoshootsAndModels with message" + ex.Message);

            }

            return photoshootsAndModels;

        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: List all photoshoots those status is SentToEditor calling from { 'photoshoots/sentToEditor/' }
        URL: /Photoshoots/SentToEditorPhotoshoot/
        Request: GET
        Input: Null
        output:  dynamic Object of Photoshoot Products
        */
        [HttpGet("SentToEditorPhotoshoot")]
        public async Task<object> SentToEditorPhotoshoot()
        {
            dynamic SendToEditorProduct = new object();
            try
            {
                SendToEditorProduct = await _PhotoshootRepo.GetSentToEditorPhotoshoot(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for SendToEditor Product with message" + ex.Message);

            }

            return SendToEditorProduct;

        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: List all photoshoots  those status is in-progress calling from { 'photoshoots/summary/' }
        URL: /Photoshoots/Summary/
        Request: GET
        Input: Null
        output:  dynamic Object of Photoshoot Products
        */
        [HttpGet("Summary")]
        public async Task<object> Summary()
        {
            dynamic PhotoshootSummary = new object();
            try
            {
                PhotoshootSummary = await _PhotoshootRepo.GetSentToEditorPhotoshoot(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for SendToEditor Product with message" + ex.Message);

            }

            return PhotoshootSummary;

        }


        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Create new photoshoot and add products in it calling from { 'photoshoots/addNewPhotoshoot/' }
        URL: /Photoshoots/create/1
        Request: POST
        Input: FromBody, string ProductId
        output: dynamic Object of Photoshoot Products
        */
        [HttpPost("create/{productId}")]
        public async Task<string> PostAsync([FromBody]   string value, string productId)
        {
            string NewInsertionID = "0";
            try
            {
                Photoshoots newPhotoshoots = JsonConvert.DeserializeObject<Photoshoots>(value);
                NewInsertionID = await _PhotoshootRepo.Create(newPhotoshoots);
                int userId = newPhotoshoots.created_by;
                event_create_photoshoot = event_create_photoshoot.Replace("%%userid%%", userId.ToString());
                event_create_photoshoot = event_create_photoshoot.Replace("%%pid%%", NewInsertionID.ToString());
                int countComma = productId.Count(c => c == ',');
                if (countComma > 0)
                {
                    var ids = productId.Split(",");  
                    foreach (var product_id in ids)
                    {
                        await _eventRepo.AddPhotoshootAsync(Int32.Parse(product_id), event_photoshoot_created_id, Int32.Parse(NewInsertionID), event_create_photoshoot, userId, _common.GetTimeStemp(), table_name);
                        await _eventRepo.AddEventAsync(event_photoshoot_created_id, userId, Int32.Parse(product_id), event_create_photoshoot, _common.GetTimeStemp(), user_event_table_name);
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Photoshoots with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Update photoshoot status to sentToPhotoshoot, shootNotStarted calling from { 'photoshoots/UpdatePhotoshootProductStatus/' }, { 'photoshoots/updateMultiplePhotoshootStatus}
        URL: /Photoshoots/UpdatePhotoshootProductStatus/
        Request: PUT
        Input: FromBody, string productId
        output: string success or failed
        */
        [HttpPut("UpdatePhotoshootProductStatus/{productId}")]
        public async Task<string> UpdatePhotoshootProductStatus(string productId, [FromBody]   string value)
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
                string PhotoshootStatus;

                int countComma = productId.Count(c => c == ',');
                if (countComma > 0)
                {
                    await _PhotoshootRepo.UpdateSpecific(ValuesToUpdate, " product_id IN ( " + productId + " ) ");
                    PhotoshootStatus = PhotoshootToUpdate.product_shoot_status_id.ToString();
                    var ids = productId.Split(",");
                    foreach (var product_id in ids)
                    {
                        int ProductId = Int32.Parse(product_id);
                        if (PhotoshootStatus == "1")
                        {
                            event_photoshoot_started = event_photoshoot_started.Replace("%%userid%%", userId.ToString());
                            event_photoshoot_started = event_photoshoot_started.Replace("%%pid%%", productId.ToString());
                            await _eventRepo.AddPhotoshootAsync(ProductId, event_photoshoot_started_id, ProductId, event_photoshoot_started, userId, _common.GetTimeStemp(), table_name);
                            await _eventRepo.AddEventAsync(event_photoshoot_started_id, userId, ProductId, event_photoshoot_started, _common.GetTimeStemp(), user_event_table_name);
                        }
                        else if (PhotoshootStatus == "2")
                        {
                            event_photoshoot_sent_to_editor = event_photoshoot_sent_to_editor.Replace("%%userid%%", userId.ToString());
                            event_photoshoot_sent_to_editor = event_photoshoot_sent_to_editor.Replace("%%pid%%", productId.ToString());
                            await _eventRepo.AddPhotoshootAsync(ProductId, event_photoshoot_sent_to_editor_id, ProductId, event_photoshoot_sent_to_editor, userId, _common.GetTimeStemp(), table_name);
                            await _eventRepo.AddEventAsync(event_photoshoot_sent_to_editor_id, userId, ProductId, event_photoshoot_sent_to_editor, _common.GetTimeStemp(), user_event_table_name);
                        }
                        else if (PhotoshootStatus == "0")
                        {
                            event_photoshoot_not_started = event_photoshoot_not_started.Replace("%%userid%%", userId.ToString());
                            event_photoshoot_not_started = event_photoshoot_not_started.Replace("%%pid%%", ProductId.ToString());
                            await _eventRepo.AddPhotoshootAsync(ProductId, event_photoshoot_not_started_id, ProductId, event_photoshoot_not_started, userId, _common.GetTimeStemp(), table_name);
                            await _eventRepo.AddEventAsync(event_photoshoot_not_started_id, userId, ProductId, event_photoshoot_not_started, _common.GetTimeStemp(), user_event_table_name);
                        }
                    }
                }
                else
                {
                    await _PhotoshootRepo.UpdateSpecific(ValuesToUpdate, " product_id = " + productId);
                    PhotoshootStatus = PhotoshootToUpdate.product_shoot_status_id.ToString();
                    int ProductId = Int32.Parse(productId);
                    if (PhotoshootStatus == "1")
                    {
                        event_photoshoot_started = event_photoshoot_started.Replace("%%userid%%", userId.ToString());
                        event_photoshoot_started = event_photoshoot_started.Replace("%%pid%%", productId.ToString());
                        await _eventRepo.AddPhotoshootAsync(ProductId, event_photoshoot_started_id, ProductId, event_photoshoot_started, userId, _common.GetTimeStemp(), table_name);
                        await _eventRepo.AddEventAsync(event_photoshoot_started_id, userId, ProductId, event_photoshoot_started, _common.GetTimeStemp(), user_event_table_name);
                    }
                    else if (PhotoshootStatus == "2")
                    {
                        event_photoshoot_sent_to_editor = event_photoshoot_sent_to_editor.Replace("%%userid%%", userId.ToString());
                        event_photoshoot_sent_to_editor = event_photoshoot_sent_to_editor.Replace("%%pid%%", productId.ToString());
                        await _eventRepo.AddPhotoshootAsync(ProductId, event_photoshoot_sent_to_editor_id, ProductId, event_photoshoot_sent_to_editor, userId, _common.GetTimeStemp(), table_name);
                        await _eventRepo.AddEventAsync(event_photoshoot_sent_to_editor_id, userId, ProductId, event_photoshoot_sent_to_editor, _common.GetTimeStemp(), user_event_table_name);
                    }
                    else if (PhotoshootStatus == "0")
                    {
                        event_photoshoot_not_started = event_photoshoot_not_started.Replace("%%userid%%", userId.ToString());
                        event_photoshoot_not_started = event_photoshoot_not_started.Replace("%%pid%%", ProductId.ToString());
                        await _eventRepo.AddPhotoshootAsync(ProductId, event_photoshoot_not_started_id, ProductId, event_photoshoot_not_started, userId, _common.GetTimeStemp(), table_name);
                        await _eventRepo.AddEventAsync(event_photoshoot_not_started_id, userId, ProductId, event_photoshoot_not_started, _common.GetTimeStemp(), user_event_table_name);
                    }
                }

                if (PhotoshootStatus == "0" || PhotoshootStatus == "1")
                {
                    await SetProductItemStatusForPhotoshoot(productId, Int32.Parse(PhotoshootStatus));
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

      
        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: First get product smallest SKU and Set Item Status For Photoshoot, its a private function calls from only in this class 
        URL:  
        Request:  
        Input: string productId, string status
        output: string success or failed
        */
        private async Task<string> SetProductItemStatusForPhotoshoot(string product_id, int status)
        {
            dynamic ToReturn = new object();
            string returnValue = "success";
            try
            {
                string sku;
                List<int> skuIdReturnList = new List<int>();
                JObject AllSkuIdsList = new JObject();

                int countComma = product_id.Count(c => c == ',');
                if (countComma > 0)
                {
                    var ids = product_id.Split(","); 
                    foreach (var productID in ids)
                    {
                        object SkuList = await _PhotoshootRepo.GetSkuByProduct(productID);
                        IEnumerable SkuListEnum = SkuList as IEnumerable;
                        List<string> skuListInt = new List<string>();
                        IDictionary<string, string> skuSortLink = new Dictionary<string, string>();
                        if (SkuListEnum != null)
                        {
                            foreach (dynamic element in SkuListEnum)
                            {
                                sku = element.sku;
                                int countDash = sku.Count(c => c == '-');
                                if (countDash > 0)
                                {
                                    skuListInt.Add(sku.Split('-').Last().ToString());
                                    skuSortLink[sku.Split('-').Last()] = element.sku_id.ToString();
                                }
                                else
                                {
                                    skuListInt.Add(element.sku_id.ToString());
                                    skuSortLink[element.sku_id.ToString()] = element.sku_id.ToString();
                                }
                            }
                            string[] skuArrayInt = skuListInt.ToArray();
                            Array.Sort(skuArrayInt);

                            List<int> SortedSkuReturnIds = new List<int>();
                            if (skuArrayInt.Count() > 0) {
                                foreach (var sku_id in skuArrayInt)
                                {
                                    string index = sku_id.ToString();
                                    SortedSkuReturnIds.Add(Int32.Parse(skuSortLink[index]));
                                }

                                var idsAll = string.Join(",", SortedSkuReturnIds.ToArray());
                                AllSkuIdsList.Add(productID, idsAll.ToString());
                            }
                            
                        }
                    }
                }
                else
                {
                    object SkuList = await _PhotoshootRepo.GetSkuByProduct(product_id);
                    IEnumerable SkuListEnum = SkuList as IEnumerable;
                    List<string> skuListInt = new List<string>();
                    IDictionary<string, string> skuSortLink = new Dictionary<string, string>();
                    if (SkuListEnum != null)
                    {
                        foreach (dynamic element in SkuListEnum)
                        {
                            sku = element.sku;
                            int countDash = sku.Count(c => c == '-');
                            if (countDash > 0)
                            {
                                skuListInt.Add(sku.Split('-').Last().ToString());
                                skuSortLink[sku.Split('-').Last()] = element.sku_id.ToString();
                            }
                            else
                            {
                                skuListInt.Add(element.sku_id.ToString());
                                skuSortLink[element.sku_id.ToString()] = element.sku_id.ToString();
                            }
                        }
                        string[] skuArraySplited = skuListInt.ToArray();
                        Array.Sort(skuArraySplited);

                        List<string> SortedSkuReturnIds = new List<string>();
                        if (skuArraySplited.Count() > 0)
                        {
                            foreach (var sku_id in skuArraySplited)
                            {
                                string index = sku_id.ToString();
                                SortedSkuReturnIds.Add(skuSortLink[index].ToString());
                            }
                            var ids = string.Join(",", SortedSkuReturnIds.ToArray());
                            AllSkuIdsList.Add(product_id, ids.ToString());
                        }
                    }
                }
                 returnValue = await _ItemServiceHelper.SetProductItemStatusForPhotoshootAsync(AllSkuIdsList.ToString(Formatting.None), status);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for SKU with message" + ex.Message);
                returnValue = "Failed";
                return returnValue;
            }
            return returnValue;
        }


        /*
        Developer: Mohi
        Date: 7-3-19
        Action: Start product photoshoot for the given photoshoot ID calling from { 'photoshoots/addProductInPhotoshoot/' } , {'photoshoots/addNewPhotoshoot'}
        URL: /Photoshoots/StartProductPhotoshoot/
        Request: POST
        Input: FromBody, string productId
        output: string success or failed
        */
        [HttpPost("StartProductPhotoshoot/{productId}")]
        public async Task<string> StartProductPhotoshoot(string productId, [FromBody]   string value)
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

                int photoShootStatus = 1;
                await SetProductItemStatusForPhotoshoot(productId, photoShootStatus);

                int countComma = productId.Count(c => c == ',');
                if (countComma > 0)
                {
                    var ids = productId.Split(",");
                    foreach (var product_id in ids)
                    {
                        await _eventRepo.AddPhotoshootAsync(Int32.Parse(product_id), event_photoshoot_started_id, photoshootId, event_photoshoot_started, userId, _common.GetTimeStemp(), table_name);
                    }
                }
                else {
                    await _eventRepo.AddPhotoshootAsync(Int32.Parse(productId), event_photoshoot_started_id, photoshootId, event_photoshoot_started, userId, _common.GetTimeStemp(), table_name);
                }
                

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  item barcode with message" + ex.Message);
                UpdateResult = "Failed";
            }

            return UpdateResult;
        }
         
    }



























}