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

        [Authorize]
        public async Task<IActionResult> Index()
        {
            SetBadgerHelper();
            ProductPhotoshootPagerList photoshootPagerList = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootPagerList>("/Photoshoots/listpageview/10");
            dynamic ProductPhotoshootModal = new ExpandoObject();
            ProductPhotoshootModal.Lists = photoshootPagerList.photoshootsInfo;
            return View("Index", ProductPhotoshootModal);

        }

        [Authorize]
        public IActionResult inProgress()
        {
           return View("InProgress");
        }

        [Authorize]
        public async Task<IActionResult> shootInProgress()
        {
            SetBadgerHelper();
            
            ProductPhotoshootInProgressPagerList photoshootInProgress         = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootInProgressPagerList>("/Photoshoots/inprogress/");
            var a = photoshootInProgress.photoshootsInprogress;

            dynamic photoshootInProgressModal   = new ExpandoObject();
            photoshootInProgressModal.Lists     = photoshootInProgress.photoshootsInprogress;

            return View("ShootInProgress", photoshootInProgressModal);
        }

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

        [Authorize]
        public async Task<IActionResult> sendToEditor()
        {
            SetBadgerHelper();
            ProductPhotoshootSendToEditorPagerList photoshootSendToEditor = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootSendToEditorPagerList>("/Photoshoots/SentToEditorPhotoshoot/");
            dynamic photoshootSendToEditorModal = new ExpandoObject();
            photoshootSendToEditorModal.Lists = photoshootSendToEditor.photoshootSendToEditor;
            return View("SendToEditor", photoshootSendToEditorModal);


            //return View("SendToEditor");
        }

        [HttpGet("photoshoots/getPhotoshootAndModels/")]
        public async Task<string> getPhotoshootAndModelsAsync()
        {
            SetBadgerHelper();
            Object photoshootPagerList = await _BadgerApiHelper.GenericGetAsync<Object>("/Photoshoots/GetPhotoshootsAndModels/");
            return photoshootPagerList.ToString() ;
        }

        [HttpGet("photoshoots/addProductInPhotoshoot/{product_id}/{photoshoot_id}")]
        public async Task<string> addProductInPhotoshoot(string product_id, int photoshoot_id)
        {
            SetBadgerHelper();

            JObject assignPhotoshoot = new JObject();
            assignPhotoshoot.Add("photoshoot_id", photoshoot_id);
            assignPhotoshoot.Add("product_shoot_status_id", 1);
            assignPhotoshoot.Add("updated_by", 2);
            assignPhotoshoot.Add("updated_at", _common.GetTimeStemp());

            String AssignPhotoshootStatus = await _BadgerApiHelper.GenericPostAsyncString<String>(assignPhotoshoot.ToString(Formatting.None), "/photoshoots/StartProductPhotoshoot/" + product_id);

            return AssignPhotoshootStatus;
        }


        [HttpGet("photoshoots/UpdatePhotoshootProductStatus/{product_id}/{status}")]
        public async Task<string> UpdatePhotoshootProductStatus(string product_id, string status)
        {
            SetBadgerHelper();
            string status_id = "";
            if (status == "NotStarted")
            {
                status_id = "0";
            }
            else if (status == "InProgress")
            {
                status_id = "1";
            }
            else if (status == "SendToEditor")
            {
                status_id = "2";
            }


            string user_id = await _ILoginHelper.GetLoginUserId();
            
            JObject photoshoot = new JObject();
            photoshoot.Add("product_shoot_status_id", status_id);
            photoshoot.Add("updated_by", user_id);
            photoshoot.Add("updated_at", _common.GetTimeStemp());

            string returnStatus = await _BadgerApiHelper.GenericPutAsyncString<string>(photoshoot.ToString(Formatting.None), "/Photoshoots/UpdatePhotoshootProductStatus/" + product_id);
            return returnStatus;
        }

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

        [Authorize]
        [HttpPost("photoshoots/updateMultiplePhotoshootStatus")]
        public async Task<String> updateMultiplePhotoshootStatus([FromBody]   JObject json)
        {
            SetBadgerHelper();
            string status_id    = "";
            string status       = json.Value<string>("status");
            string product_id   = json.Value<string>("product_id");

            if (status == "NotStarted")
            {
                status_id = "0";
            }
            else if (status == "InProgress")
            {
                status_id = "1";
            }
            else if (status == "SendToEditor")
            {
                status_id = "2";
            }

            string user_id = await _ILoginHelper.GetLoginUserId();

            JObject photoshoot = new JObject();
            photoshoot.Add("product_shoot_status_id", status_id);
            photoshoot.Add("updated_by", user_id);
            photoshoot.Add("updated_at", _common.GetTimeStemp());

            string returnStatus = await _BadgerApiHelper.GenericPutAsyncString<string>(photoshoot.ToString(Formatting.None), "/Photoshoots/UpdatePhotoshootProductStatus/" + product_id);
            return returnStatus;
        }
    }
}