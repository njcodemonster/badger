using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using GenericModals.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections;
using badgerApi.Helper;
using GenericModals.Event;
using GenericModals;
using GenericModals.Extentions;

namespace badgerApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PhotoshootsController : ControllerBase
    {
        private IPhotoshootRepository _PhotoshootRepo;
        private IItemServiceHelper _ItemServiceHelper;
        private INotesAndDocHelper _NotesAndDoc;
        private int note_type = 1;

        ILoggerFactory _loggerFactory;
        private IEventRepo _eventRepo;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        string product_event_table_name = "product_events";
        string user_event_table_name = "user_events";

        string photoshoot_created = "photoshoot_created";
        string product_photoshoot_created = "product_photoshoot_created";
        string photoshoot_started = "photoshoot_started";
        string photoshoot_not_started = "photoshoot_not_started";
        string photoshoot_sent_to_editor = "photoshoot_sent_to_editor";
        string photoshoot_note_created = "photoshoot_note_created";
        string photoshoot_summmary_updated = "photoshoot_summmary_updated";

        public PhotoshootsController(IPhotoshootRepository PhotoshootRepo, ILoggerFactory loggerFactory, IEventRepo eventRepo, IItemServiceHelper ItemServiceHelper, INotesAndDocHelper NotesAndDoc)
        {
            _ItemServiceHelper = ItemServiceHelper;
            _eventRepo = eventRepo;
            _PhotoshootRepo = PhotoshootRepo;
            _loggerFactory = loggerFactory;
            _NotesAndDoc = NotesAndDoc;
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

        [HttpGet("inprogress/{photoshootId}")]
        public async Task<ResponseModel> GetInprogress(int photoshootId)
        {
            var response = await _PhotoshootRepo.GetInprogressPhotoshootById(photoshootId);
            return ResponseHelper.GetResponse(response);
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
        public async Task<ResponseModel> listpageviewAsync(int limit)
        {
            try
            {
                var productList = await _PhotoshootRepo.GetPhotoshootDetailsRep(limit);
                productList.Where(x => x.po_status == "Not Recieved").ToList().ForEach(x => x.po_status = "Expected " + x.expected_date);
                return ResponseHelper.GetResponse(productList);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);
                return new ResponseModel { Message = ex.Message, Status = HttpStatusCode.InternalServerError };
            }
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
        public async Task<ResponseModel> SentToEditorPhotoshoot()
        {
            try
            {
                var productList = await _PhotoshootRepo.GetSentToEditorPhotoshoot(0);
                productList.Where(x => x.po_status == "Not Recieved").ToList().ForEach(x => x.po_status = "Expected " + x.expected_date);
                return ResponseHelper.GetResponse(productList);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for SendToEditor Product with message" + ex.Message);
                return new ResponseModel { Message = ex.Message, Status = HttpStatusCode.InternalServerError };
            }
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
                PhotoshootSummary = await _PhotoshootRepo.GetPhotoshootSummary();
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for Summary Page with message" + ex.Message);

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
        [HttpPost("create")]
        public async Task<ResponseModel> PostAsync([FromBody] Photoshoots photoshoot)
        {
            string NewInsertionID = "0";
            try
            {
                NewInsertionID = _PhotoshootRepo.Create(photoshoot);
                int userId = photoshoot.created_by;

                foreach (var product_id in photoshoot.products.Select(x => x.product_id))
                {
                    int prodId = product_id;
                    await _eventRepo.AddEventAsync(new EventModel(product_event_table_name) { EventName = photoshoot_created, EntityId = prodId, RefrenceId = Int32.Parse(NewInsertionID), UserId = userId });
                    await _eventRepo.AddEventAsync(new EventModel(user_event_table_name) { EventName = photoshoot_created, EntityId = userId, RefrenceId = prodId, UserId = userId, EventNoteId = prodId });
                }
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Photoshoots with message" + ex.Message);
            }
            return ResponseHelper.GetResponse(NewInsertionID);
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
                            await _eventRepo.AddEventAsync(new EventModel(product_event_table_name) { EventName = photoshoot_started, EntityId = ProductId, RefrenceId = 0, UserId = userId });
                            await _eventRepo.AddEventAsync(new EventModel(user_event_table_name) { EventName = photoshoot_started, EntityId = userId, RefrenceId = ProductId, UserId = userId, EventNoteId = ProductId });
                        }
                        else if (PhotoshootStatus == "2")
                        {
                            await _eventRepo.AddEventAsync(new EventModel(product_event_table_name) { EventName = photoshoot_sent_to_editor, EntityId = ProductId, RefrenceId = ProductId, UserId = userId });
                            await _eventRepo.AddEventAsync(new EventModel(user_event_table_name) { EventName = photoshoot_sent_to_editor, EntityId = userId, RefrenceId = ProductId, UserId = userId, EventNoteId = ProductId });
                        }
                        else if (PhotoshootStatus == "0")
                        {
                            await _eventRepo.AddEventAsync(new EventModel(product_event_table_name) { EventName = photoshoot_not_started, EntityId = ProductId, RefrenceId = ProductId, UserId = userId });
                            await _eventRepo.AddEventAsync(new EventModel(user_event_table_name) { EventName = photoshoot_not_started, EntityId = userId, RefrenceId = ProductId, UserId = userId, EventNoteId = ProductId });
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
                        await _eventRepo.AddEventAsync(new EventModel(product_event_table_name) { EventName = photoshoot_started, EntityId = ProductId, RefrenceId = 0, UserId = userId });
                        await _eventRepo.AddEventAsync(new EventModel(user_event_table_name) { EventName = photoshoot_started, EntityId = userId, RefrenceId = ProductId, UserId = userId, EventNoteId = ProductId });
                    }
                    else if (PhotoshootStatus == "2")
                    {
                        await _eventRepo.AddEventAsync(new EventModel(product_event_table_name) { EventName = photoshoot_sent_to_editor, EntityId = ProductId, RefrenceId = ProductId, UserId = userId });
                        await _eventRepo.AddEventAsync(new EventModel(user_event_table_name) { EventName = photoshoot_sent_to_editor, EntityId = userId, RefrenceId = ProductId, UserId = userId, EventNoteId = ProductId });
                    }
                    else if (PhotoshootStatus == "0")
                    {
                        await _eventRepo.AddEventAsync(new EventModel(product_event_table_name) { EventName = photoshoot_not_started, EntityId = ProductId, RefrenceId = ProductId, UserId = userId });
                        await _eventRepo.AddEventAsync(new EventModel(user_event_table_name) { EventName = photoshoot_not_started, EntityId = userId, RefrenceId = ProductId, UserId = userId, EventNoteId = ProductId });
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
                            if (skuArrayInt.Count() > 0)
                            {
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
        [HttpPost("StartProductPhotoshoot")]
        public async Task<ResponseModel> StartProductPhotoshoot([FromBody] ProductPhotoshoots photoshootToUpdate)
        {
            string UpdateResult = "Success";
            try
            {
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
                ValuesToUpdate.Add("photoshoot_id", photoshootToUpdate.photoshoot_id.ToString());
                ValuesToUpdate.Add("product_shoot_status_id", photoshootToUpdate.product_shoot_status_id.ToString());
                ValuesToUpdate.Add("updated_by", photoshootToUpdate.updated_by.ToString());
                ValuesToUpdate.Add("updated_at", photoshootToUpdate.updated_at.ToString());
                var productIds = photoshootToUpdate.products.Select(x => x.product_id);
                await _PhotoshootRepo.UpdateSpecific(ValuesToUpdate, "product_id IN (" + string.Join(',', productIds) + ")");

                int userId = photoshootToUpdate.updated_by;
                int photoshootId = photoshootToUpdate.photoshoot_id;
                string productId = string.Empty;
                int photoShootStatus = 1;

                // var smallestSkuList = await _PhotoshootRepo.GetSmallestSkuByProduct(productIds.ToList());
                var result = await _ItemServiceHelper.PostAsync<string>(photoshootToUpdate.items, "/item/UpdateProductItemForPhotoshoot/" + photoShootStatus);

                var eventModels = GenerateEventModels(productIds, userId, photoshootId);
                await _eventRepo.AddEventAsync(eventModels);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  item barcode with message" + ex.Message);
                UpdateResult = "Failed";
            }

            return ResponseHelper.GetResponse(UpdateResult);
        }

        private IEnumerable<EventModel> GenerateEventModels(IEnumerable<int> productIds, int userId, int photoshootId)
        {
            foreach (var product_id in productIds)
            {
                yield return new EventModel(product_event_table_name) { EventName = photoshoot_started, EntityId = product_id, RefrenceId = photoshootId, UserId = userId };
            }
        }


        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Update photoshoot fields photoshoot name,   calling from { 'photoshoots/UpdatePhotoshootProductStatus/' }, { 'photoshoots/updateMultiplePhotoshootStatus}
        URL: /Photoshoots/UpdatePhotoshootProductStatus/
        Request: PUT
        Input: FromBody, string productId
        output: string success or failed
        */
        [HttpPut("UpdatePhotoshootSummary/{photoshootId}")]
        public async Task<string> UpdatePhotoshootSummary(int photoshootId, [FromBody]   string value)
        {
            string UpdateResult = "Success";
            try
            {
                JObject PhotoshootToUpdate = JsonConvert.DeserializeObject<JObject>(value);
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();

                ValuesToUpdate.Add("photoshoot_name", PhotoshootToUpdate.Value<string>("photoshoot_name"));
                ValuesToUpdate.Add("model_id", PhotoshootToUpdate.Value<string>("model_id"));
                ValuesToUpdate.Add("shoot_start_date", PhotoshootToUpdate.Value<string>("shoot_start_date"));
                ValuesToUpdate.Add("updated_by", PhotoshootToUpdate.Value<string>("updated_by"));
                ValuesToUpdate.Add("updated_at", PhotoshootToUpdate.Value<string>("updated_at"));

                int userId = PhotoshootToUpdate.Value<int>("updated_by");
                await _PhotoshootRepo.UpdatePhotoshootForSummary(ValuesToUpdate, " photoshoot_id = " + photoshootId.ToString());

                await _eventRepo.AddEventAsync(new EventModel(product_event_table_name) { EventName = photoshoot_summmary_updated, EntityId = photoshootId, RefrenceId = photoshootId, UserId = userId });
                await _eventRepo.AddEventAsync(new EventModel(user_event_table_name) { EventName = photoshoot_summmary_updated, EntityId = userId, RefrenceId = photoshootId, UserId = userId,EventNoteId = photoshootId });
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for SKU with message" + ex.Message);
                UpdateResult = "Failed";
                return UpdateResult;
            }
            return UpdateResult;

        }


        /*
       Developer: Mohi
       Date: 7-16-19 
       Action: Create photoshoot notes 
       URL: api/photoshoot/notecreate
       Request: Post
       Input: FromBody, string 
       output: string of Last insert photoshoot note id
       */
        [HttpPost("notecreate")]
        public async Task<string> NoteCreate([FromBody]   string value)
        {
            string newNoteID = "0";
            try
            {
                dynamic photoshootNote = JsonConvert.DeserializeObject<Object>(value);
                int ref_id = photoshootNote.ref_id;
                string note = photoshootNote.note;
                int created_by = photoshootNote.created_by;
                double created_at = _common.GetTimeStemp();
                newNoteID = await _NotesAndDoc.GenericPostNote<string>(ref_id, note_type, note, created_by, created_at);

                int userId = photoshootNote.created_by;
                await _eventRepo.AddEventAsync(new EventModel(product_event_table_name) { EventName = photoshoot_note_created, EntityId = ref_id, RefrenceId = ref_id, UserId = userId });
                await _eventRepo.AddEventAsync(new EventModel(user_event_table_name) { EventName = photoshoot_note_created, EntityId = userId, RefrenceId = ref_id, UserId = userId, EventNoteId = ref_id });
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Photoshoot Note with message" + ex.Message);
            }
            return newNoteID;
        }

        /*
        Developer: Mohi
        Date: 7-17-19 
        Action: Get photoshoot note by note id and limit  
        URL: api/photoshoot/getnote/ref_id/limit
        Request: Get
        Input: int ref_id, int limit 
        output: List of Photoshoot note
        */
        [HttpGet("getnote/{ref_id}/{limit}")]
        public async Task<List<Notes>> GetNoteViewAsync(int ref_id, int limit)
        {
            List<Notes> notes = new List<Notes>();
            try
            {
                notes = await _NotesAndDoc.GenericNote<Notes>(ref_id, note_type, limit);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for photoshoot Get Note with message" + ex.Message);

            }

            return notes;

        }

        /*
        Developer: Mohi
        Date: 7-17-19 
        Action: Get notes of Photoshoot Note by multiple ids with comma seperate
        URL: api/photoshoot/getitemnotes/ref_ids
        Request: Get
        Input: string ids
        output: string of photoshoot orders note id 
        */
        [HttpGet("getitemnotes/{ref_ids}")]
        public async Task<List<Notes>> GetItemNotesViewAsync(string ref_ids)
        {
            List<Notes> notes = new List<Notes>();
            try
            {
                notes = await _NotesAndDoc.GenericNotes<Notes>(ref_ids, note_type);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for photoshoot Notes with message" + ex.Message);

            }
            return notes;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Create new photoshoot product 
        URL: /Photoshoots/addNewPhotoshootProduct/1
        Request: POST
        Input: FromBody
        output: string success or failed
        */
        [HttpPost("addNewPhotoshootProduct/")]
        public async Task<string> addNewPhotoshootProduct([FromBody]   string value)
        {
            string Result = "success";
            try
            {
                ProductPhotoshoots newPhotoshoots = JsonConvert.DeserializeObject<ProductPhotoshoots>(value);
                await _PhotoshootRepo.CreatePhotoshootProduct(newPhotoshoots);
                int userId = newPhotoshoots.created_by;
                int productId = newPhotoshoots.product_id;

                await _eventRepo.AddEventAsync(new EventModel(product_event_table_name) { EventName = product_photoshoot_created, EntityId = productId, RefrenceId = 0, UserId = userId });
                await _eventRepo.AddEventAsync(new EventModel(user_event_table_name) { EventName = product_photoshoot_created, EntityId = userId, RefrenceId = productId, UserId = userId, EventNoteId = productId });
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in add new Photoshoots product with message" + ex.Message);
                Result = "failed";
            }
            return Result;
        }

        [HttpPost("smallestitem")]
        public async Task<ResponseModel> GetSmallestSkus([FromBody] List<SmallestItem> items)
        {
            var response = await _PhotoshootRepo.GetSmallestSkuByProduct(items.Select(x => x.product_id).ToList());
            var lastItem = await _ItemServiceHelper.PostAsync<List<SmallestItem>>(response, $"/item/smallestitem");
            return ResponseHelper.GetResponse(lastItem);
        }

        [HttpPost("bulkupdatebarcode")]
        public async Task<ResponseModel> BulkUpdateBarcode([FromBody] List<BarcodeUpdate> items)
        {
            var item = await _ItemServiceHelper.PostAsync<string>(items, $"/item/bulkupdatebarcode");
            return ResponseHelper.GetResponse(item);
        }
    }

}