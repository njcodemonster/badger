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

namespace badger_view.Controllers
{
    public class PhotoshootsController : Controller
    {

        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        private CommonHelper _CommonHelper;
        public PhotoshootsController(IConfiguration config)
        {
            _config = config;

        }
        private void SetCommonHelper()
        {
            if (_CommonHelper == null)
            {
                _CommonHelper = new CommonHelper(_config);
            }
        }
        private void SetBadgerHelper()
        {
            if (_BadgerApiHelper == null)
            {
                _BadgerApiHelper = new BadgerApiHelper(_config);
            }
        }

        public async Task<IActionResult> Index()
        {/**
            
               @foreach (ProductPhotoshootRep photoshoot in Model.Lists)
            {

                <tr style="">
                    <td class="text-center">
                        <input type="checkbox" class="select-box">
                    </td>
                    <td><img style="width:85px;" src="@photoshoot.product_vendor_image"></td>
                    <td>@photoshoot.vendor_name</td>
                    <td></td>
                    <td><b>-</b></td>
                    <td><b>@photoshoot.sku_family</b></td>
                    <td></td>
                    <td>
                        <select class="browser-default custom-select">
                            <option selected>Shoot not started</option>
                            <option >Add to photoshoot</option>
                        </select>
                    </td>
                </tr>
            }



            SetBadgerHelper();
            ProductPhotoshootPagerList photoshootPagerList = await _BadgerApiHelper.GenericGetAsync<ProductPhotoshootPagerList>("/Photoshoots/listpageview/10");
            dynamic ProductPhotoshootModal = new ExpandoObject();
            ProductPhotoshootModal.Lists = photoshootPagerList.photoshootsInfo;
            return View("Index", ProductPhotoshootModal);*/

            return View("Index");
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

        public string getPhotoshootAndModels()
        {
            return "{\"photoshootsList\":[{\"photoshoot_id\":9001,\"photoshoot_name\":\"Parker - 1 / 2 / 55\"},{\"photoshoot_id\":9002,\"photoshoot_name\":\"Alexis Parker -4 / 5 / 19\"}],\"photoshootsModelList\":[{\"model_id\":1,\"model_name\":\"Alexis Parker\"},{\"model_id\":2,\"model_name\":\"Liberty Muttal\"},{\"model_id\":3,\"model_name\":\"Amanda Stanton\"}]}";
        }

        [HttpGet("photoshoots/addProductInPhotoshoot/{product_id}/{photoshoot_id}")]
        public async Task<string> addProductInPhotoshoot(int product_id, int photoshoot_id)
        {
            string print = product_id.ToString() + " - " + photoshoot_id.ToString();
            return print;
        }

        
        /*[HttpGet("photoshoots/addNewPhotoshoot/{product_id}")]
        public async Task<string> addNewPhotoshoot(int product_id)
        {
            string print = product_id.ToString() ;
            return print;
        }*/

        [HttpPost("photoshoots/addNewPhotoshoot")]
        public async Task<String> addNewPhotoshoot([FromBody]   JObject json)
        {
            SetBadgerHelper();
            SetCommonHelper();
            /*
            JObject photoshoot = new JObject();
            photoshoot.Add("photoshoot_name", json.Value<string>("photoshoot_name"));
            photoshoot.Add("model_id", json.Value<int>("model_id"));
            photoshoot.Add("shoot_start_date", json.Value<double>("shoot_start_date"));
            photoshoot.Add("shoot_end_date", json.Value<double>("shoot_end_date"));
            photoshoot.Add("active_status", 1);
            photoshoot.Add("created_by", 2);
            photoshoot.Add("updated_by", 2);
            photoshoot.Add("created_at", _CommonHelper.GetTimeStemp());
            photoshoot.Add("updated_at", _CommonHelper.GetTimeStemp());
            String newPhotoshootID = await _BadgerApiHelper.GenericPostAsyncString<String>(photoshoot.ToString(Formatting.None), "/photoshoots/create");
            */
            String newPhotoshootID = "9004";
            string productId = json.Value<string>("product_id");

            JObject assignPhotoshoot = new JObject();
            assignPhotoshoot.Add("product_id", productId);
            assignPhotoshoot.Add("photoshoot_id", newPhotoshootID);
            assignPhotoshoot.Add("product_shoot_status_id", 1);
            assignPhotoshoot.Add("updated_by", 2);
            assignPhotoshoot.Add("updated_at", _CommonHelper.GetTimeStemp());

            String AssignPhotoshootStatus = await _BadgerApiHelper.GenericPostAsyncString<String>(assignPhotoshoot.ToString(Formatting.None), "/photoshoots/assignProductPhotoshoot");

            return newPhotoshootID;
        }

        }
}