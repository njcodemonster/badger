using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using badger_view.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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

        public BarcodeController(IConfiguration config, ILoginHelper LoginHelper)
        {
            _LoginHelper = LoginHelper;
            _config = config;
            UploadPath = _config.GetValue<string>("UploadPath:path");
            S3bucket = _config.GetValue<string>("S3config:Bucket_Name");
            S3folder = _config.GetValue<string>("S3config:Folder");

        }
        private void SetBadgerHelper()
        {
            if (_BadgerApiHelper == null)
            {
                _BadgerApiHelper = new BadgerApiHelper(_config);
            }
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
            SetBadgerHelper();

            dynamic vendorPagerList = await _BadgerApiHelper.GenericGetAsync<object>("/Barcode/getBarcodeRange/0/0");

            dynamic VendorPageModal = new ExpandoObject();
            VendorPageModal.VendorCount = vendorPagerList.Count;
            VendorPageModal.VendorLists = vendorPagerList.vendorInfo;
            VendorPageModal.VendorType = vendorPagerList.vendorType;

            return View("Index", VendorPageModal);
        }
    }
}