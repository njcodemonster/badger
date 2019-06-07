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
namespace badger_view.Controllers
{
    public class PhotoshootsController : Controller
    {

        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public PhotoshootsController(IConfiguration config)
        {
            _config = config;

        }
       
        private void SetBadgerHelper()
        {
            if (_BadgerApiHelper == null)
            {
                _BadgerApiHelper = new BadgerApiHelper(_config);
            }
        }

        public async Task<IActionResult> Index()
        {
            SetBadgerHelper();
            ProductPhotoshootPagerList photoshootPagerList = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootPagerList>("/Photoshoots/listpageview/10");
            dynamic ProductPhotoshootModal = new ExpandoObject();
            ProductPhotoshootModal.Lists = photoshootPagerList.photoshootsInfo;
            return View("Index", ProductPhotoshootModal);

        }

        public IActionResult inProgress()
        {
            return View("inprogress");
        }

        public IActionResult shootInProgress(int photoshootId)
        {
            return View("shootInProgress");
        }

        public IActionResult sendToEditor(int photoshootId)
        {
            return View("sendToEditor");
        }

        [HttpGet("photoshoots/getPhotoshootAndModels/")]
        public async Task<string> getPhotoshootAndModelsAsync()
        {
            //photoshootsAndModels
            SetBadgerHelper();
            Object photoshootPagerList = await _BadgerApiHelper.GenericGetAsync<Object>("/Photoshoots/photoshootsAndModels/");
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

            String AssignPhotoshootStatus = await _BadgerApiHelper.GenericPostAsyncString<String>(assignPhotoshoot.ToString(Formatting.None), "/photoshoots/assignProductPhotoshoot/" + product_id);

            return AssignPhotoshootStatus;
        }

        [HttpPost("photoshoots/addNewPhotoshoot")]
        public async Task<String> addNewPhotoshoot([FromBody]   JObject json)
        {
            SetBadgerHelper();
           

            JObject photoshoot = new JObject();
            photoshoot.Add("photoshoot_name", json.Value<string>("photoshoot_name"));
            photoshoot.Add("model_id", json.Value<int>("model_id"));
            photoshoot.Add("shoot_start_date", json.Value<double>("shoot_start_date"));
            photoshoot.Add("shoot_end_date", json.Value<double>("shoot_end_date"));
            photoshoot.Add("active_status", 1);
            photoshoot.Add("created_by", 2);
            photoshoot.Add("updated_by", 2);
            photoshoot.Add("created_at", _common.GetTimeStemp());
            photoshoot.Add("updated_at", _common.GetTimeStemp());
            String newPhotoshootID = await _BadgerApiHelper.GenericPostAsyncString<String>(photoshoot.ToString(Formatting.None), "/photoshoots/create");
            
            string productId = json.Value<string>("product_id");

            JObject assignPhotoshoot = new JObject();
            //assignPhotoshoot.Add("product_id", productId);
            assignPhotoshoot.Add("photoshoot_id", newPhotoshootID);
            assignPhotoshoot.Add("product_shoot_status_id", 1);
            assignPhotoshoot.Add("updated_by", 2);
            assignPhotoshoot.Add("updated_at", _common.GetTimeStemp());

            String AssignPhotoshootStatus = await _BadgerApiHelper.GenericPostAsyncString<String>(assignPhotoshoot.ToString(Formatting.None), "/photoshoots/assignProductPhotoshoot/"+ productId);

            return AssignPhotoshootStatus;
        }

        }
}