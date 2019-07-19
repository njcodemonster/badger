using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using badgerApi.Helper;
using badgerApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderManagementController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IPurchaseOrdersRepository _PurchaseOrdersRepo;
        ILoggerFactory _loggerFactory;
        private INotesAndDocHelper _NotesAndDoc;
        private int note_type = 6;
        private IItemServiceHelper _ItemsHelper;

        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();

        private string userEventTableName = "user_events";
        private string tableName = "item_events";

        private int event_item_note_id = 29;
        private int event_item_document_id = 30;

        private string event_create_item_notecreate = "Item note created by user =%%userid%% with note id= %%noteid%%";
        private string event_create_item_documentcreate = "Item document created by user =%%userid%% with document id= %%noteid%%";

        IEventRepo _eventRepo;
        public PurchaseOrderManagementController(IPurchaseOrdersRepository PurchaseOrdersRepo, ILoggerFactory loggerFactory, INotesAndDocHelper NotesAndDoc, IConfiguration config, IItemServiceHelper ItemsHelper, IEventRepo eventRepo)
        {
            _eventRepo = eventRepo;
            _config = config;
            _PurchaseOrdersRepo = PurchaseOrdersRepo;
            _loggerFactory = loggerFactory;
            _NotesAndDoc = NotesAndDoc;
            _ItemsHelper = ItemsHelper;
        }

        /*
       Developer: Sajid Khan
       Date: 7-13-19 
       Action: Find Sku from Lin item array
       URL: /api/purchaseordermanagement/FindSku/LineItemArray/sku
       Request: Get
       Input: dynamic LineItemArray,String sku
       output: int number count which matched
       */
        private int FindSku(dynamic LineItemArray,String sku)
        {
            int index = -1;
            foreach(dynamic LineItem in LineItemArray)
            {
                if(sku == (String)LineItem.sku)
                {
                    index++;
                    break;
                }
                index++;
            }
            return index;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: All item status of Purchase Order Management
        URL: /api/purchaseordermanagement/ListAllItemStatus
        Request: Get
        Input: Null
        output: dynamic object of purchase orders items status
        */
        [HttpGet("ListAllItemStatus")]
        public async Task<object> ListAllItemStatus()
        {
            dynamic AllItemStatus = new object();
            try
            {
                AllItemStatus = await _ItemsHelper.GetAllStatus();
            }
            catch (Exception ex)
            {   
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for all Item status from Item service with message" + ex.Message);
            }
            return AllItemStatus;

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get All items of Purchase Order Management
        URL: /api/purchaseordermanagement/GetLineItemDetails/poid/limit
        Request: Get
        Input: int poid, int limit
        output: dynamic object of purchase orders items 
        */
        [HttpGet("GetLineItemDetails/{PO_id}/{limit}")]
        public async Task<object> GetLineItemsDetails(int PO_id, int limit)
        {
            dynamic LineITemsDetails = new object();
            try
            {
                LineITemsDetails = await _PurchaseOrdersRepo.GetOpenPOLineItemDetails(PO_id,limit);
                List<Items> items = await _ItemsHelper.GetItemsByOrder(PO_id);
                List<Items> skuItems = new List<Items>();
                string currentSKU = "";
                Boolean First = true;
                int Index = -1;
                foreach (Items item in items)
                {
                    if(item.sku != currentSKU)
                    {
                        
                        if (!First)
                        {
                            Index = FindSku(LineITemsDetails.LineItemDetails, currentSKU);
                            if(Index != -1)
                            {
                                LineITemsDetails.LineItemDetails[Index].EndItems = skuItems;
                            }
                        }
                        currentSKU = item.sku;
                        First = false;
                        skuItems = new List<Items>();
                    }
                    skuItems.Add(item);
                   

                }
                Index = FindSku(LineITemsDetails.LineItemDetails, currentSKU);
                if (Index != -1)
                {
                    LineITemsDetails.LineItemDetails[Index].EndItems = skuItems;
                }
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetLineItemsDetails for PO : "+PO_id.ToString()+" with message" + ex.Message);
            }

            return LineITemsDetails;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Create Purchase Order Note
        URL: api/purchaseordermanagement/notecreate
        Request: Post
        Input: Frombody string
        output: string of purchase orders note id 
        */
        [HttpPost("notecreate")]
        public async Task<string> NoteCreate([FromBody]   string value)
        {
            string newNoteID = "0";
            try
            {
                dynamic newItemNote = JsonConvert.DeserializeObject<Object>(value);
                int ref_id = newItemNote.ref_id;
                string note = newItemNote.note;
                int created_by = newItemNote.created_by;
                double created_at = _common.GetTimeStemp();
                newNoteID = await _NotesAndDoc.GenericPostNote<string>(ref_id, note_type, note, created_by, created_at);

                event_create_item_notecreate = event_create_item_notecreate.Replace("%%userid%%", created_by.ToString()).Replace("%%noteid%%", newNoteID);

                _eventRepo.AddItemEventAsync(ref_id,0, event_item_note_id, Int32.Parse(newNoteID), event_create_item_notecreate, created_by, _common.GetTimeStemp(), tableName);

                _eventRepo.AddEventAsync(event_item_note_id, created_by, Int32.Parse(newNoteID), event_create_item_notecreate, _common.GetTimeStemp(), userEventTableName);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Item Note with message" + ex.Message);
            }
            return newNoteID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get item note of Purchase Order Note by multiple ids with comma seperate
        URL: api/purchaseordermanagement/getitemnotes/ref_ids
        Request: Get
        Input: string ids
        output: string of purchase orders note id 
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
                logger.LogInformation("Problem happened in selecting the data for item Get Notes with message" + ex.Message);

            }
            return notes;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get item document note of Purchase Order Note
        URL: api/purchaseordermanagement/getitemdocuments/ref_id/limit
        Request: Get
        Input: int ref_id, int limit
        output: List of purchase orders document
        */
        [HttpGet("getitemdocuments/{ref_id}/{limit}")]
        public async Task<List<Documents>> GetDocumentsViewAsync(int ref_id, int limit)
        {
            List<Documents> documents = new List<Documents>();
            try
            {
                documents = await _NotesAndDoc.GenericGetDocAsync<Documents>(ref_id, note_type, limit);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for item Get documents with message" + ex.Message);

            }

            return documents;

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Create New of Purchase Order document
        URL: api/purchaseordermanagement/documentcreate
        Request: Post
        Input: int ref_id, int limit
        output: List of purchase orders document
        */
        [HttpPost("documentcreate")]
        public async Task<string> DocumentCreate([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                dynamic ItemToUpdate = JsonConvert.DeserializeObject<JObject>(value);

                int ref_id = ItemToUpdate.ref_id;
                string document_url = ItemToUpdate.url;
                int created_by = ItemToUpdate.created_by;
                double created_at = _common.GetTimeStemp();

                NewInsertionID = await _NotesAndDoc.GenericPostDoc<string>(ref_id, note_type, document_url, "", created_by, created_at);

                event_create_item_documentcreate = event_create_item_documentcreate.Replace("%%userid%%", created_by.ToString()).Replace("%%documentid%%", NewInsertionID);

                _eventRepo.AddItemEventAsync(ref_id, 0, event_item_document_id, Int32.Parse(NewInsertionID), event_create_item_documentcreate, created_by, _common.GetTimeStemp(), tableName);

                _eventRepo.AddEventAsync(event_item_document_id, created_by, Int32.Parse(NewInsertionID), event_create_item_documentcreate, _common.GetTimeStemp(), userEventTableName);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Purchase Order Document create with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Item update by id 
        URL: api/purchaseordermanagement/itemupdate/id
        Request: Post
        Input: int id, Frombody string
        output: string of purchase orders item
        */
        [HttpPost("itemupdate/{id}")]
        public async Task<string> ItemUpdate(int id,[FromBody] string value)
        {
            string ItemUpdate = "0";
            try
            {
                ItemUpdate = await _ItemsHelper.ItemUpdateById(id,value); 
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Purchase Order Document create with message" + ex.Message);
            }
            return ItemUpdate;
        }

        /*
        Developer: Sajid Khan
        Date: 7-16-19 
        Action: All Ra status of Purchase Order Management
        URL: /api/purchaseordermanagement/ListAllRaStatus
        Request: Get
        Input: Null
        output: dynamic object of purchase orders ra status
        */
        [HttpGet("ListAllRaStatus")]
        public async Task<object> ListAllRaStatus()
        {
            dynamic AllRaStatus = new object();
            try
            {
                AllRaStatus = await _PurchaseOrdersRepo.GetAllRaStatus();
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for all ra status with message" + ex.Message);
            }
            return AllRaStatus;

        }

        /*
        Developer: Sajid Khan
        Date: 7-16-19 
        Action: All Wash Types
        URL: /api/purchaseordermanagement/ListAllWashTypes
        Request: Get
        Input: Null
        output: dynamic list of all Wash Types
        */
        [HttpGet("ListAllWashTypes")]
        public async Task<object> ListAllWashTypes()
        {
            dynamic AllRaStatus = new object();
            try
            {
                AllRaStatus = await _PurchaseOrdersRepo.GetAllWashTypes();
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for all ra status with message" + ex.Message);
            }
            return AllRaStatus;

        }

    }
}