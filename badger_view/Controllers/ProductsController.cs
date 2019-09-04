using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using badger_view.Helpers;
using GenericModals.Models;
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

            productDetailsPageData = await _BadgerApiHelper.GenericGetAsync<ProductDetailsPageData>("/Product/detailpage/" + id);
            //  dynamic AttributeListDetails = new ExpandoObject();
            //  VendorPageModal.VendorCount = vendorPagerList.Count;
            //  VendorPageModal.VendorLists = vendorPagerList.vendorInfo;
            // VenderAdressandRep venderAdressandRep = await _BadgerApiHelper.GenericGetAsync<VenderAdressandRep>("/Vendor/detailsaddressandrep/103");

            //VendorPageModal.Reps = venderAdressandRep.Reps;
            return View("EditAttributes", productDetailsPageData);
        }

        [Authorize]
        [HttpPost("product/UpdateAttributes")]
        public async Task<IActionResult> UpdateAttributes([FromBody]   JObject json)
        {
            ProductDetailsPageData productDetailsPageData = new ProductDetailsPageData();
            return View("EditAttributes", productDetailsPageData);
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
            JArray productDetailArray = new JArray();
            string messageDocuments = "";
            List<IFormFile> files = productFiles.productImages;
            string loginUserId = await _LoginHelper.GetLoginUserId();
            try
            {
                foreach (var formFile in files)
                {

                    if (formFile.Length > 0)
                    {


                        JObject productImageDetails = new JObject();
                        awsS3Helper.UploadToS3(formFile.FileName, formFile.OpenReadStream(), S3bucket, S3folder);

                        int product_id = Int32.Parse(productFiles.product_id);
                        JObject productDocuments = new JObject();
                        string product_title = System.IO.Path.GetFileNameWithoutExtension(formFile.FileName);
                        productDocuments.Add("product_id", product_id);
                        productDocuments.Add("product_image_title", product_title);
                        productDocuments.Add("product_image_url", formFile.FileName);
                        productDocuments.Add("isprimary", Int32.Parse(productFiles.product_primary));
                        productDocuments.Add("created_at", 0);
                        productDocuments.Add("updated_at", 0);
                        productDocuments.Add("created_by", Int32.Parse(loginUserId));
                        string img_id = await _BadgerApiHelper.GenericPostAsyncString<String>(productDocuments.ToString(Formatting.None), "/product/createProductImage");
                        productImageDetails.Add("image_id", img_id);
                        productImageDetails.Add("product_name", formFile.FileName);
                        productDetailArray.Add(productImageDetails);
                    }
                }
                string data = JsonConvert.SerializeObject(productDetailArray);
                return data;
            }
            catch (Exception ex)
            {
                return "0";
            }
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
        [HttpPost("/product/UpdateProductImagePrimary")]
        public async Task<string> UpdateProductImagePrimary([FromBody]   JObject json)
        {
            SetBadgerHelper();
            string result = "0";
            try
            {
                JObject imageData = JObject.Parse(json.ToString());
                JArray imageDataArray = (JArray)imageData["dataImage"];

                for (int i = 0; i < imageDataArray.Count; i++)
                {
                    JObject imageObj = new JObject();
                    imageObj.Add("product_img_id", imageDataArray[i].Value<string>("product_img_id"));
                    imageObj.Add("is_primary", imageDataArray[i].Value<string>("is_primary"));
                    result = await _BadgerApiHelper.GenericPostAsyncString<String>(imageObj.ToString(Formatting.None), "/product/updateProductImagePrimary");
                }
                return result;
            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        /*
        Developer: Hamza Haq
        Date: 9-03-19
        Request: GET
        Action:Get vendor Products for autocomplete
        URL: /product/autosuggest/{vendor_id}/{productname}
        Input: Vendor ID and product name
        output: productList
        */
        [Authorize]
        [HttpGet("product/autosuggest/{vendor_id}/{productname}")]
        public async Task<string> Autosuggest(int vendor_id, string productname)
        {
            SetBadgerHelper();

            var  ProductList = await _BadgerApiHelper.GenericGetAsync<List<object>>("/product/getProductsbyVendor/" + vendor_id + "/" + productname);
            return JsonConvert.SerializeObject(ProductList);
        }
    }
}