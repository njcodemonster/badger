using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using badger_view.Helpers;
using GenericModals.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace badger_view.Controllers
{
    public class BarcodeController : Controller
    {
        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        private CommonHelper.awsS3helper awsS3Helper = new CommonHelper.awsS3helper();
        private String UploadPath = "";
        private String S3bucket = "";
        private String S3folder = "";
        private ILoginHelper _LoginHelper;

        public BarcodeController(IConfiguration config, ILoginHelper LoginHelper, BadgerApiHelper badgerApiHelper)
        {
            _LoginHelper = LoginHelper;
            _config = config;
            UploadPath = _config.GetValue<string>("UploadPath:path");
            S3bucket = _config.GetValue<string>("S3config:Bucket_Name");
            S3folder = _config.GetValue<string>("S3config:Folder");
            _BadgerApiHelper = badgerApiHelper;
        }

         /*
         Developer: Rizwan ali
         Date: 7-3-19 
         Action: getting all barcode ranges from badger api
         URL: index
         Input: Null
         output: dynamic ExpandoObject of Barcoderanges
        */
        [Authorize]
        public async Task<IActionResult> Index()
        {

            dynamic barcodeRanges = await _BadgerApiHelper.GenericGetAsync<List<Barcode>>("/Barcode/getBarcodeRange/0/0");
            return View("Index", barcodeRanges);
        }
        /*
         Developer: Rizwan ali
         Date: 7-3-19 
         Action: getting all barcode ranges from badger api
         URL: index
        Input: Null
        output: dynamic ExpandoObject of Barcoderanges
        */
        [HttpPost("/barcode/validate")]
        public async Task<bool> ValidateBarcode([FromBody]   JObject json)
        {
            string isValidate = string.Empty;
            // Validate Barcode Range
            JObject bar = new JObject();
            string barcode_from = json.Value<string>("barcode_from");
            string barcode_to = json.Value<string>("barcode_to");
            string size = json.Value<string>("size");
            string idStr = json.Value<string>("id");
            if(idStr == "")
            {
                idStr = "-1";
            }
            Int32 id = int.Parse(idStr);
            bar.Add("id", id);
            bar.Add("size", size);
            bar.Add("barcode_from", barcode_from);
            bar.Add("barcode_to", barcode_to);
            isValidate = await _BadgerApiHelper.GenericPostAsyncString<string>(bar.ToString(Formatting.None), "/Barcode/validate");

            return Convert.ToBoolean(isValidate);
        }

        /*
         Developer: Rizwan ali
         Date: 7-3-19 
         Action: getting all barcode ranges from badger api
         URL: index
         Input: Null
         output: dynamic ExpandoObject of Barcoderanges
        */
        [HttpPost("/barcode/create")]
        public async Task<bool> CreateBarcode([FromBody]   JObject json)
        {
            string inserted = string.Empty;
            // Add/Update Barcode Range
            JObject bar = new JObject();
            string barcode_from = json.Value<string>("barcode_from");
            string barcode_to = json.Value<string>("barcode_to");
            string size = json.Value<string>("size");
            string idStr = json.Value<string>("id");
            //string updatedBy = await _LoginHelper.GetLoginUserId();
            if (idStr == "")
            {
                idStr = "-1";
            }
            Int32 id = int.Parse(idStr);
            bar.Add("id", id);
            bar.Add("size", size);
            bar.Add("barcode_from", barcode_from);
            bar.Add("barcode_to", barcode_to);
            //bar.Add("updated_by",Convert.ToInt32(updatedBy));
            //bar.Add("created_by", Convert.ToInt32(updatedBy));
            //bar.Add("updated_at", _common.DateConvertToTimeStamp(DateTime.Now.ToString()));
            inserted = await _BadgerApiHelper.GenericPostAsyncString<string>(bar.ToString(Formatting.None), "/barcode/createorupdate");

            return Convert.ToBoolean(inserted);
        }
        /*
         Developer: Rizwan ali
         Date: 7-3-19 
         Action: getting all barcode ranges from badger api
         URL: index
         Input: Null
         output: dynamic ExpandoObject of Barcoderanges
        */
        [HttpPost("/barcode/delete/{id}")]
        public async Task<bool> DeleteBarcode(int id)
        {
            bool deleted = false;
            
            deleted = await _BadgerApiHelper.GenericGetAsync<bool>("/barcode/deletebarcode/"+id);

            return Convert.ToBoolean(deleted);
        }
    }
}