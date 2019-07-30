using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using badger_view.Helpers;
using badger_view.Models;
using System.Dynamic;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace badger_view.Controllers
{
    public class productFileData
    {
        public List<IFormFile> productImages { get; set; }
        public string product_id { get; set; }
        public string product_title { get; set; }
        public string product_primary { get; set; }
    }
    public class ProductsController : Controller
    {
        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        private CommonHelper.awsS3helper awsS3Helper = new CommonHelper.awsS3helper();
        private String UploadPath = "";
        private String S3bucket = "";
        private String S3folder = "";
        private ILoginHelper _LoginHelper;
        public ProductsController(IConfiguration config, ILoginHelper LoginHelper)
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
            Developer: Azeem Hassan
            Date: 7-3-19
            Request: GET
            Action:send attributes id to badger api to get attributes
            URL: product/EditAttributes/id
            Input: attributes id
            output: view data in EditAttributes
        */
        [Authorize]
        [HttpGet("product/EditAttributes/{id}")]
        public async Task<IActionResult> EditAttributes(string id)
        {

            ProductDetailsPageData productDetailsPageData = new ProductDetailsPageData();

            SetBadgerHelper();

            productDetailsPageData = await _BadgerApiHelper.GenericGetAsync<ProductDetailsPageData>("/Product/detailpage/"+id);
            //  dynamic AttributeListDetails = new ExpandoObject();
            //  VendorPageModal.VendorCount = vendorPagerList.Count;
            //  VendorPageModal.VendorLists = vendorPagerList.vendorInfo;
            // VenderAdressandRep venderAdressandRep = await _BadgerApiHelper.GenericGetAsync<VenderAdressandRep>("/Vendor/detailsaddressandrep/103");

            //VendorPageModal.Reps = venderAdressandRep.Reps;
            return View("EditAttributes",productDetailsPageData );
        }

        [Authorize]
        [HttpPost("product/UpdateAttributes")]
        public async Task<IActionResult> UpdateAttributes([FromBody]   JObject json)
        {
            ProductDetailsPageData productDetailsPageData = new ProductDetailsPageData();
            return View("EditAttributes", productDetailsPageData);
        }

        [Authorize]
        [HttpPost("/product/InsertattributeImages")]
        public async Task<string> InsertattributeImages(productFileData productFiles)
        {
            string messageDocuments = "";
            List<IFormFile> files = productFiles.productImages;
            try
            {
                foreach (var formFile in files)
                {
                    //if (formFile.Length > 0)
                    //{

                    //    awsS3Helper.UploadToS3(formFile.FileName, formFile.OpenReadStream(), S3bucket, S3folder);
                    //    messageDocuments = "image upload";
                    //}
                    if (formFile.Length > 0)
                    {
                        string Fill_path = formFile.FileName;
                        Fill_path = UploadPath + Fill_path;
                        if (System.IO.File.Exists(Fill_path))
                        {
                            messageDocuments += "File Already Exists: " + Fill_path + " \r\n";
                        }
                        else
                        {
                            using (var stream = new FileStream(Fill_path, FileMode.Create))
                            {
                                messageDocuments += Fill_path + " \r\n";

                                //awsS3Helper.UploadToS3(formFile.FileName, formFile.OpenReadStream(), S3bucket, S3folder);
                                await formFile.CopyToAsync(stream);
                                int product_id = Int32.Parse(productFiles.product_id);
                                JObject productDocuments = new JObject();
                                productDocuments.Add("product_id", product_id);
                                productDocuments.Add("product_image_url", Fill_path);
                                productDocuments.Add("product_image_title", productFiles.product_title);
                                productDocuments.Add("isprimary", Int32.Parse(productFiles.product_primary));
                                productDocuments.Add("created_at", 0);
                                productDocuments.Add("updated_at", 0);
                                messageDocuments = await _BadgerApiHelper.GenericPostAsyncString<String>(productDocuments.ToString(Formatting.None), "/product/createProductImage");

                            }
                        }
                    }
                }
                return messageDocuments;
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
    }
}