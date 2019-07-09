using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using badger_view.Helpers;
using badger_view.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CommonHelper;
using Microsoft.AspNetCore.Authorization;
namespace badger_view.Controllers
{
    public class PhotoshootsController : Controller
    {
        private ILoginHelper _ILoginHelper;
        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public PhotoshootsController(IConfiguration config, ILoginHelper LoginHelper)
        {
            _ILoginHelper = LoginHelper; 
            _config = config;

        }
       
        private void SetBadgerHelper()
        {
            if (_BadgerApiHelper == null)
            {
                _BadgerApiHelper = new BadgerApiHelper(_config);
            }
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
            SetBadgerHelper();
            ProductPhotoshootPagerList photoshootPagerList = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootPagerList>("/Photoshoots/listpageview/10");
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
        public async Task<IActionResult> shootInProgress()
        {
            SetBadgerHelper();
            
            ProductPhotoshootInProgressPagerList photoshootInProgress         = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootInProgressPagerList>("/Photoshoots/inprogress/");
            
            dynamic photoshootInProgressModal   = new ExpandoObject();
            photoshootInProgressModal.Lists     = photoshootInProgress.photoshootsInprogress;

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
            SetBadgerHelper();
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
            SetBadgerHelper();
            ProductPhotoshootSendToEditorPagerList photoshootSendToEditor = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootSendToEditorPagerList>("/Photoshoots/SentToEditorPhotoshoot/");
            dynamic photoshootSendToEditorModal = new ExpandoObject();
            photoshootSendToEditorModal.Lists = photoshootSendToEditor.photoshootSendToEditor;
            return View("SendToEditor", photoshootSendToEditorModal);
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
            SetBadgerHelper();
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
            SetBadgerHelper();
            ProductPhotoshootSendToEditorPagerList photoshootSummary = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootSendToEditorPagerList>("/Photoshoots/summary/");
            dynamic photoshootSummaryModal = new ExpandoObject();
            photoshootSummaryModal.Lists = photoshootSummary.photoshootSendToEditor;
            return View("summary", photoshootSummaryModal);
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
        [HttpGet("photoshoots/addProductInPhotoshoot/{product_id}/{photoshoot_id}")]
        public async Task<string> addProductInPhotoshoot(string product_id, int photoshoot_id)
        {
            SetBadgerHelper();
            string user_id = await _ILoginHelper.GetLoginUserId();

            JObject assignPhotoshoot = new JObject();
            assignPhotoshoot.Add("photoshoot_id", photoshoot_id);
            assignPhotoshoot.Add("product_shoot_status_id", 1);
            assignPhotoshoot.Add("updated_by", user_id);
            assignPhotoshoot.Add("updated_at", _common.GetTimeStemp());

            String AssignPhotoshootStatus = await _BadgerApiHelper.GenericPostAsyncString<String>(assignPhotoshoot.ToString(Formatting.None), "/photoshoots/StartProductPhotoshoot/" + product_id);

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
            SetBadgerHelper();
              
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
        public async Task<String> addNewPhotoshoot([FromBody]   JObject json)
        {
            SetBadgerHelper();
            string user_id = await _ILoginHelper.GetLoginUserId();

            JObject photoshoot = new JObject();
            photoshoot.Add("photoshoot_name", json.Value<string>("photoshoot_name"));
            photoshoot.Add("model_id", json.Value<int>("model_id"));
            photoshoot.Add("shoot_start_date", json.Value<double>("shoot_start_date"));
            photoshoot.Add("shoot_end_date", json.Value<double>("shoot_end_date"));
            photoshoot.Add("active_status", 1);
            photoshoot.Add("created_by", user_id);
            photoshoot.Add("updated_by", 0);
            photoshoot.Add("created_at", _common.GetTimeStemp());
            photoshoot.Add("updated_at", 0);

            string productId = json.Value<string>("product_id");

            String newPhotoshootID = await _BadgerApiHelper.GenericPostAsyncString<String>(photoshoot.ToString(Formatting.None), "/photoshoots/create/"+ productId);
            

            JObject assignPhotoshoot = new JObject();
            //assignPhotoshoot.Add("product_id", productId);
            assignPhotoshoot.Add("photoshoot_id", newPhotoshootID);
            assignPhotoshoot.Add("product_shoot_status_id", 1);
            assignPhotoshoot.Add("updated_by", user_id);
            assignPhotoshoot.Add("updated_at", _common.GetTimeStemp());

            String AssignPhotoshootStatus = await _BadgerApiHelper.GenericPostAsyncString<String>(assignPhotoshoot.ToString(Formatting.None), "/photoshoots/StartProductPhotoshoot/" + productId);

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
            SetBadgerHelper(); 
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
            SetBadgerHelper();
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

            String returnStatus = await _BadgerApiHelper.GenericPostAsyncString<String>(photoshootModel.ToString(Formatting.None), "/PhotoshootModels/create/");

            return returnStatus;
        }
    }
}