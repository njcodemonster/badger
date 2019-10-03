using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using badger_view.Helpers;
using GenericModals.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CommonHelper;
using Microsoft.AspNetCore.Authorization;
using GenericModals;

namespace badger_view.Controllers
{
    public class PhotoshootsController : Controller
    {
        private ILoginHelper _ILoginHelper;
        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public PhotoshootsController(IConfiguration config, ILoginHelper LoginHelper, BadgerApiHelper badgerApiHelper)
        {
            _ILoginHelper = LoginHelper; 
            _config = config;
            _BadgerApiHelper = badgerApiHelper;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: List all Photoshoot Products which are not started from this api url "/Photoshoots/listpageview/10"
        URL: /Photoshoots
        Input: Null
        output: dynamic ExpandoObject of ProductPhotoshoot
        */
        [Authorize]
        public async Task<IActionResult> Index()
        {
            ProductPhotoshootPagerList photoshootPagerList = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootPagerList>("/Photoshoots/listpageview/0");
            dynamic ProductPhotoshootModal = new ExpandoObject();
            ProductPhotoshootModal.Lists = photoshootPagerList.photoshootsInfo;
            return View("Index", ProductPhotoshootModal);

        }


        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: List all Photoshoots which are in-progress from this api url "/Photoshoots/inprogress/"
        URL: /Photoshoots/shootInProgress
        Input: Null
        output: dynamic ExpandoObject of ProductPhotoshoot
        */
        [Authorize]
        [HttpGet("photoshoots/shootInProgress/{selectPhotoshootId}")]
        public async Task<IActionResult> shootInProgress(string selectPhotoshootId)
        {
            ProductPhotoshootInProgressPagerList photoshootInProgress         = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootInProgressPagerList>("/Photoshoots/inprogress/");
            
            dynamic photoshootInProgressModal   = new ExpandoObject();
            photoshootInProgressModal.Lists     = photoshootInProgress.photoshootsInprogress;
            photoshootInProgressModal.SelectedPhotoshoot = selectPhotoshootId;

            return View("ShootInProgress", photoshootInProgressModal);
        }

        [Authorize]
        [HttpGet("photoshoots/shootInProgress/")]
        public async Task<IActionResult> shootInProgress()
        {
            ProductPhotoshootInProgressPagerList photoshootInProgress = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootInProgressPagerList>("/Photoshoots/inprogress/");
            string selectPhotoshootId = "0";
            dynamic photoshootInProgressModal = new ExpandoObject();
            photoshootInProgressModal.Lists = photoshootInProgress.photoshootsInprogress;
            photoshootInProgressModal.SelectedPhotoshoot = selectPhotoshootId;

            return View("ShootInProgress", photoshootInProgressModal);
        }


        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: get all photoshoot products by photoshoot id which are in-progress from this api url "/Photoshoots/GetPhotoshootsProducts/"
        URL: /Photoshoots/getPhotoshootInProgressProducts/1
        Input: Photoshoot Id
        output: dynamic ExpandoObject of PhotoshootInProgressProducts
        */
        [Authorize]
        [HttpGet("photoshoots/getPhotoshootInProgressProducts/{photoshootId}")]
        public async Task<IActionResult> getPhotoshootInProgressProducts(int photoshootId)
        {
            string callUrl = "/Photoshoots/GetPhotoshootsProducts/" + photoshootId;
            ProductPhotoshootPagerList photoshootPagerList = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootPagerList>(callUrl);
            dynamic ProductPhotoshootModal = new ExpandoObject();
            ProductPhotoshootModal.Lists = photoshootPagerList.photoshootsInfo;
            ProductPhotoshootModal.PhotoshootID = photoshootId;
            return View("InProgressPhotoshootProductsViewAjax", ProductPhotoshootModal);
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: List all photoshoot products those are SentToEditor from this api url "/Photoshoots/SentToEditorPhotoshoot/"
        URL: /Photoshoots/sentToEditor/
        Input: Null
        output: dynamic ExpandoObject of Photoshoot SentToEditor Products
        */
        [Authorize]
        public async Task<IActionResult> sentToEditor()
        {
            ProductPhotoshootSendToEditorPagerList photoshootSendToEditor = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootSendToEditorPagerList>("/Photoshoots/SentToEditorPhotoshoot/");
            dynamic photoshootSendToEditorModal = new ExpandoObject();
            photoshootSendToEditorModal.Lists = photoshootSendToEditor.photoshootSendToEditor;
            return View("SentToEditor", photoshootSendToEditorModal);
        }


        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: All photoshoot and models from this api url "/Photoshoots/GetPhotoshootsAndModels/"
        URL: /Photoshoots/getPhotoshootAndModels/
        Input: Null
        output: Json string of photoshoot and models 
        */
        [Authorize]
        [HttpGet("photoshoots/getPhotoshootAndModels/")]
        public async Task<string> getPhotoshootAndModelsAsync()
        {
            Object photoshootPagerList = await _BadgerApiHelper.GenericGetAsync<Object>("/Photoshoots/GetPhotoshootsAndModels/");
            return photoshootPagerList.ToString() ;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: List all photoshoots from this api url "/Photoshoots/summary/"
        URL: /Photoshoots/summary/
        Input: Null
        output: dynamic ExpandoObject of Photoshoot Summary
        */
        [Authorize]
        public async Task<IActionResult> summary()
        {
            ProductPhotoshootSummaryPagerList photoshootSummary = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootSummaryPagerList>("/Photoshoots/summary/");
            dynamic allPhotoshootModels = await _BadgerApiHelper.GenericGetAsync<dynamic>("/PhotoshootModels/list");
            dynamic photoshootSummaryModal = new ExpandoObject();
            photoshootSummaryModal.Lists = photoshootSummary.productPhotoshootSummary;
            photoshootSummaryModal.PhotoshootModels = allPhotoshootModels;
            return View("Summary", photoshootSummaryModal);
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Create JObject to add product in photoshoot using this api url "/Photoshoots/StartProductPhotoshoot/{product_id}"
        URL: /Photoshoots/addProductInPhotoshoot/1/100
        Input: ProductID, PhotoshootID
        output: string success or failed
        */
        [Authorize]
        [HttpPost("photoshoots/addProductInPhotoshoot/{photoshoot_id}")]
        public async Task<string> addProductInPhotoshoot(int photoshoot_id, [FromBody] PhotoshootWithItems photoshoot)
        {
            string user_id = await _ILoginHelper.GetLoginUserId();

            var assignPhotoshoot = new ProductPhotoshoots();
            assignPhotoshoot.photoshoot_id = photoshoot_id;
            assignPhotoshoot.product_shoot_status_id = 1;
            assignPhotoshoot.updated_by =int.Parse(user_id);
            assignPhotoshoot.updated_at = _common.GetTimeStemp();
            assignPhotoshoot.products = photoshoot.products;
            assignPhotoshoot.items = photoshoot.items;
            // await _BadgerApiHelper.PostAsync<string>(photoshoot.items, "/photoshoots/bulkupdatebarcode");
            var AssignPhotoshootStatus = await _BadgerApiHelper.PostAsync<string>(assignPhotoshoot, "/photoshoots/StartProductPhotoshoot");
            
            return AssignPhotoshootStatus;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Update product photoshoot status for single product using this api url "/Photoshoots/UpdatePhotoshootProductStatus/{product_id}"
        URL: /Photoshoots/addProductInPhotoshoot/1/NotStarted
        Input: ProductID, Status
        output: string success or failed
        */
        [Authorize]
        [HttpGet("photoshoots/UpdatePhotoshootProductStatus/{product_id}/{status}")]
        public async Task<string> UpdatePhotoshootProductStatus(string product_id, string status)
        { 
            string user_id = await _ILoginHelper.GetLoginUserId();
            
            JObject photoshoot = new JObject();
            photoshoot.Add("product_shoot_status_id", status);
            photoshoot.Add("updated_by", user_id);
            photoshoot.Add("updated_at", _common.GetTimeStemp());

            string returnStatus = await _BadgerApiHelper.GenericPutAsyncString<string>(photoshoot.ToString(Formatting.None), "/Photoshoots/UpdatePhotoshootProductStatus/" + product_id);
            return returnStatus;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Create JObject of photoshoot to add new and add product in that photoshoot using this api url "/Photoshoots/create/{product_id}" and "/photoshoots/StartProductPhotoshoot/{product_id}"
        URL: /Photoshoots/addNewPhotoshoot/1/NotStarted
        Input: FromBody   
        output: string success or failed
        */
        [Authorize]
        [HttpPost("photoshoots/addNewPhotoshoot")]
        public async Task<String> addNewPhotoshoot([FromBody] Photoshoots photoshootDto)
        {
            string user_id = await _ILoginHelper.GetLoginUserId();

            photoshootDto.active_status = 1;
            photoshootDto.created_by = int.Parse(user_id);
            photoshootDto.created_at = _common.GetTimeStemp();
            photoshootDto.updated_at = 0;
            photoshootDto.updated_by = 0;

            string productId = photoshootDto.products.ToString();

            String newPhotoshootID = await _BadgerApiHelper.PostAsync<String>(photoshootDto, "/photoshoots/create");
            

            var assignPhotoshoot = new ProductPhotoshoots();
            //assignPhotoshoot.Add("product_id", productId);
            assignPhotoshoot.photoshoot_id = int.Parse(newPhotoshootID);
            assignPhotoshoot.product_shoot_status_id = 1;
            assignPhotoshoot.updated_by = int.Parse(user_id);
            assignPhotoshoot.updated_at = _common.GetTimeStemp();
            assignPhotoshoot.products = photoshootDto.products;
            assignPhotoshoot.items = photoshootDto.items;

            String AssignPhotoshootStatus = await _BadgerApiHelper.PostAsync<String>(assignPhotoshoot, "/photoshoots/StartProductPhotoshoot");

            return AssignPhotoshootStatus;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Update product photoshoot status for multiple products using this api url "/Photoshoots/UpdatePhotoshootProductStatus/{product_id}"
        URL: /Photoshoots/addProductInPhotoshoot/1/NotStarted
        Input: FromBody
        output: string success or failed
        */
        [Authorize]
        [HttpPost("photoshoots/updateMultiplePhotoshootStatus")]
        public async Task<String> updateMultiplePhotoshootStatus([FromBody]   JObject json)
        { 
            string status       = json.Value<string>("status");
            string product_id   = json.Value<string>("product_id");
             
            string user_id = await _ILoginHelper.GetLoginUserId();

            JObject photoshoot = new JObject();
            photoshoot.Add("product_shoot_status_id", status);
            photoshoot.Add("updated_by", user_id);
            photoshoot.Add("updated_at", _common.GetTimeStemp());

            string returnStatus = await _BadgerApiHelper.GenericPutAsyncString<string>(photoshoot.ToString(Formatting.None), "/Photoshoots/UpdatePhotoshootProductStatus/" + product_id);
            return returnStatus;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Create JObject of photoshoot to add new photoshoot using this api url "/Photoshoots/create/" 
        URL: /Photoshoots/addNewPhotoshootModel/
        Input: FromBody   
        output: string success or failed
        */
        [Authorize]
        [HttpPost("photoshoots/addNewPhotoshootModel")]
        public async Task<String> addNewPhotoshootModel([FromBody]   JObject json)
        {
            string user_id = await _ILoginHelper.GetLoginUserId();

            JObject photoshootModel = new JObject();
            photoshootModel.Add("model_name", json.Value<string>("model_name"));
            photoshootModel.Add("model_height", json.Value<string>("model_height"));
            photoshootModel.Add("model_hair", json.Value<string>("model_hair"));
            photoshootModel.Add("model_ethnicity", json.Value<string>("model_ethnicity"));

            photoshootModel.Add("active_status", 1);
            photoshootModel.Add("created_by", user_id);
            photoshootModel.Add("updated_by", 0);
            photoshootModel.Add("created_at", _common.GetTimeStemp());
            photoshootModel.Add("updated_at", 0);

            //dynamic photoshootNotes = await _BadgerApiHelper.GenericGetAsync<Object>("/photoshoots/getnote/" + photoshootId.ToString() + "/1");

            String returnStatus = await _BadgerApiHelper.GenericPostAsyncString<String>(photoshootModel.ToString(Formatting.None), "/PhotoshootModels/create/");

            return returnStatus;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Update photoshoot model, schedule date 
        URL: /Photoshoots/EditPhotoshootSummary/
        Input: FromBody   
        output: string success or failed
        */
        [Authorize]
        [HttpPost("photoshoots/EditPhotoshootSummary")]
        public async Task<String> EditPhotoshootSummary([FromBody]   JObject json)
        {
            string user_id = await _ILoginHelper.GetLoginUserId();

            JObject photoshoot = new JObject();
            photoshoot.Add("photoshoot_name", json.Value<string>("photoshootName"));
            photoshoot.Add("model_id", json.Value<int>("photoshootModelId"));
            photoshoot.Add("shoot_start_date", json.Value<double>("photoshootScheduledDate"));
            photoshoot.Add("updated_by", user_id);
            photoshoot.Add("updated_at", _common.GetTimeStemp());
            int photoshootId = Int32.Parse(json.Value<string>("photoshootId"));

            String returnStatus = await _BadgerApiHelper.GenericPutAsyncString<String>(photoshoot.ToString(Formatting.None), "/photoshoots/UpdatePhotoshootSummary/" + photoshootId);
            return returnStatus;
        }

        /*
        Developer: Mohi
        Date: 7-3-19 
        Action: Add photoshoot notes
        URL: /Photoshoots/AddPhotoshootNotes/
        Input: FromBody   
        output: string success or failed
        */
        [Authorize]
        [HttpPost("photoshoots/AddPhotoshootNotes")]
        public async Task<String> AddPhotoshootNotes([FromBody]   JObject json)
        {
            string user_id = await _ILoginHelper.GetLoginUserId();
            int photoshootId = Int32.Parse(json.Value<string>("photoshootId"));
            string returnStatus = "0";

            if (json.Value<string>("photoshootNotes") != null)
            {
                JObject photoshootNote = new JObject();
                photoshootNote.Add("ref_id", photoshootId);
                photoshootNote.Add("note", json.Value<string>("photoshootNotes"));
                photoshootNote.Add("created_by", Int32.Parse(user_id));

                returnStatus = await _BadgerApiHelper.GenericPostAsyncString<String>(photoshootNote.ToString(Formatting.None), "/photoshoots/notecreate");
            }

            return returnStatus;
        }

        /*
        Developer: Mohi
        Date: 7-17-19 
        Action: Get photoshoot notes by multiple ids by using badger api helper 
        URL: /photoshoot/getitemnotes/id
        Request: Get
        Input: int id
        output: string of purchase orders item notes
        */
        [Authorize]
        [HttpGet("photoshoots/getphotoshootnotes/{ids}")]
        public async Task<String> GetPhotoshootNotes(string ids)
        {
            dynamic photoshootNote = await _BadgerApiHelper.GenericGetAsync<Object>("/photoshoots/getitemnotes/" + ids.ToString());

            return JsonConvert.SerializeObject(photoshootNote);
        }

        [Authorize]
        [HttpGet("photoshoots/smallestitem/{po_id}/{vendor_id}/{product_id}")]
        public async Task<IActionResult> GetSmallestItem(int po_id, int vendor_id, int product_id)
        {
            var response = await _BadgerApiHelper.GetAsync<SmallestItem>($"/photoshoots/smallestitem/{po_id}/{vendor_id}/{product_id}");
            return Ok(response);
        }

        [Authorize]
        [HttpPost("photoshoots/smallestitem")]
        public async Task<IActionResult> GetSmallestSkus([FromBody] List<SmallestItem> items)
        {
            var response = await _BadgerApiHelper.PostAsync<List<SmallestItem>>(items, $"/photoshoots/smallestitem");
            return Ok(response);
        }

    }
}