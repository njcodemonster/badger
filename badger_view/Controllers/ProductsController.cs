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
        [HttpPost("product/UpdateAttributes/{productId}")]
        public async Task<string> UpdateAttributes([FromBody]   JObject json, int productId)
        {
            SetBadgerHelper();
            string user_id = await _LoginHelper.GetLoginUserId();

            JObject product = new JObject();
            product.Add("size_and_fit_id", json.Value<string>("size_fit"));
            product.Add("product_retail", json.Value<string>("product_retail"));
            product.Add("product_name", json.Value<string>("product_name"));
            product.Add("product_cost", json.Value<string>("product_cost"));
            product.Add("product_discount", json.Value<string>("product_discount"));
            product.Add("product_detail_1", json.Value<string>("product_detail_1"));
            product.Add("product_detail_2", json.Value<string>("product_detail_2"));
            product.Add("product_detail_3", json.Value<string>("product_detail_3"));
            product.Add("product_detail_4", json.Value<string>("product_detail_4"));
            product.Add("updated_by", user_id);
            product.Add("updated_at", _common.GetTimeStemp());
            
            string returnStatus = await _BadgerApiHelper.GenericPutAsyncString<string>(product.ToString(Formatting.None), "/Product/UpdateAttributes/" + productId.ToString());
 
            if (json.Value<string>("oldInternalNotes") == json.Value<string>("internalNotes"))
            {
            }
            else
            {
                JObject productNote = new JObject();
                productNote.Add("ref_id", productId);
                productNote.Add("note", json.Value<string>("internalNotes"));
                productNote.Add("created_by", Int32.Parse(user_id));

                await _BadgerApiHelper.GenericPostAsyncString<String>(productNote.ToString(Formatting.None), "/product/notecreate");
            }
          


            return "success";
        }
        /*
           Developer: Azeem Hassan
           Date: 7-30-19
           Request: POST
           Action:send images to badger api to insert attr images
           URL: /product/InsertattributeImages
           Input: image data
           output: massage
       */
        [Authorize]
        [HttpPost("/product/InsertattributeImages")]
        public async Task<string> InsertattributeImages(productFileData productFiles)
        {
            SetBadgerHelper();
            string messageDocuments = "";
            List<IFormFile> files = productFiles.productImages;
            string loginUserId = await _LoginHelper.GetLoginUserId();
            try
            {
                foreach (var formFile in files)
                {
                   
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
                                productDocuments.Add("product_image_title", productFiles.product_title);
                                productDocuments.Add("product_image_url", Fill_path);                                
                                productDocuments.Add("isprimary", Int32.Parse(productFiles.product_primary));
                                productDocuments.Add("created_at", 0);
                                productDocuments.Add("updated_at", 0);
                                productDocuments.Add("created_by", Int32.Parse(loginUserId));
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