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
using GenericModals;
using Microsoft.Extensions.Options;

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
        private ProductHelper _ProductHelper;
        private AppSettings _appSettings;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        private CommonHelper.awsS3helper awsS3Helper = new CommonHelper.awsS3helper();
        private String UploadPath = "";
        private String S3bucket = "";
        private String S3folder = "";
        private ILoginHelper _LoginHelper;
        public ProductsController(IConfiguration config, ILoginHelper LoginHelper, BadgerApiHelper badgerApiHelper)
        {

            _LoginHelper = LoginHelper;
            _config = config;
            UploadPath = _config.GetValue<string>("UploadPath:path");
            S3bucket = _config.GetValue<string>("S3config:Bucket_Name");
            S3folder = _config.GetValue<string>("S3config:Folder");
            _BadgerApiHelper = badgerApiHelper;
            _ProductHelper = new ProductHelper(_config);
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
            productDetailsPageData = await _BadgerApiHelper.GenericGetAsync<ProductDetailsPageData>("/Product/detailpage/" + id);
            //  dynamic AttributeListDetails = new ExpandoObject();
            //  VendorPageModal.VendorCount = vendorPagerList.Count;
            //  VendorPageModal.VendorLists = vendorPagerList.vendorInfo;
            // VenderAdressandRep venderAdressandRep = await _BadgerApiHelper.GenericGetAsync<VenderAdressandRep>("/Vendor/detailsaddressandrep/103");

            //VendorPageModal.Reps = venderAdressandRep.Reps;
            return View("EditAttributes", productDetailsPageData);
        }

        [Authorize]
        [HttpPost("product/UpdateAttributes/{productId}")]
        public async Task<string> UpdateAttributes([FromBody]   JObject json, int productId)
        {
            JArray product_subtype_ids = new JArray();
            string user_id = await _LoginHelper.GetLoginUserId();
            List<Fabric> _fabrics = new List<Fabric>();
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
            product.Add("is_ready", json.Value<bool>("is_ready"));
            product.Add("pairProductIds", json.Value<string>("pairProductIds"));
            product.Add("RemovePairWithProductIds", json.Value<string>("RemovePairWithProductIds"));
            product.Add("otherColorsProductIds", json.Value<string>("otherColorsProductIds"));
            product.Add("RemoveOtherColorProductIds", json.Value<string>("RemoveOtherColorProductIds"));


            product.Add("photoshootStatus", json.Value<string>("photoshootStatus"));
            product.Add("photoshootStatusOld", json.Value<string>("photoshootStatusOld"));

            product.Add("updated_by", user_id);
            product.Add("updated_at", _common.GetTimeStemp());

            _fabrics=json.Value<JArray>("fabricArray").ToObject<List<Fabric>>();
            

            string returnStatus = await _BadgerApiHelper.GenericPutAsyncString<string>(product.ToString(Formatting.None), "/Product/UpdateAttributes/" + productId.ToString());

            if (json["product_subtype_ids"] != null)
            {
                product_subtype_ids = (JArray)json["product_subtype_ids"];
            }
            for (int i = 0; i < product_subtype_ids.Count(); i++)
            {
                string category_id = product_subtype_ids[i].Value<string>("category_id");
                string action = product_subtype_ids[i].Value<string>("action");

                JObject productCategories = new JObject();
                productCategories.Add("product_id", productId);
                productCategories.Add("category_id", category_id);
                productCategories.Add("action", action);
                productCategories.Add("created_by", user_id);
                productCategories.Add("created_at", _common.GetTimeStemp());

                var temp_product_category_id = await _BadgerApiHelper.GenericPostAsyncString<String>(productCategories.ToString(Formatting.None), "/product/UpdateProductCategory");

            }
            string sku = "";
            string tagAddedIds = json.Value<string>("tagAddedIds");

            if (tagAddedIds != "")
            {
                int countComma = tagAddedIds.Count(c => c == ',');
                if (countComma > 0)
                {
                    var ids = tagAddedIds.Split(",");
                    foreach (var tagId in ids)
                    {
                        JObject product_attr = new JObject();
                        Int16 attribute_id = Int16.Parse(tagId);

                        product_attr.Add("attribute_id", attribute_id);
                        product_attr.Add("product_id", productId);
                        product_attr.Add("value", "");

                        product_attr.Add("created_by", user_id);
                        product_attr.Add("created_at", _common.GetTimeStemp());
                        String attr_value_id = await _BadgerApiHelper.GenericPostAsyncString<String>(product_attr.ToString(Formatting.None), "/attributevalues/create");


                        JObject product_attr_value = new JObject();
                        product_attr_value.Add("product_id", productId);
                        product_attr_value.Add("attribute_id", attribute_id);
                        product_attr_value.Add("value_id", attr_value_id);

                        product_attr_value.Add("created_by", user_id);
                        // product_attr_value.Add("created_at", _common.GetTimeStemp()); need to create in DB
                        String product_attribute_value_id = await _BadgerApiHelper.GenericPostAsyncString<String>(product_attr_value.ToString(Formatting.None), "/product/createAttributesValues");

                        JObject product_attribute_obj = new JObject();
                        product_attribute_obj.Add("product_id", productId);
                        product_attribute_obj.Add("attribute_id", attribute_id);
                        product_attribute_obj.Add("value_id", attr_value_id);
                        product_attribute_obj.Add("sku", sku);

                        product_attribute_obj.Add("created_by", user_id);
                        //product_attribute_obj.Add("created_at", _common.GetTimeStemp()); need to create in DB
                        String product_attribute_id = await _BadgerApiHelper.GenericPostAsyncString<String>(product_attribute_obj.ToString(Formatting.None), "/product/createProductAttribute");


                    }
                }
                else
                {

                }
            }
            if (_fabrics.Count() > 0)
            {
                await _ProductHelper.UpdateFabric(_fabrics, int.Parse(user_id));
            }



            if (json.Value<string>("photoshootStatus") != json.Value<string>("photoshootStatusOld"))
            {
                JObject photoshoot = new JObject();
                photoshoot.Add("product_shoot_status_id", json.Value<string>("photoshootStatus"));
                photoshoot.Add("updated_by", user_id);
                photoshoot.Add("updated_at", _common.GetTimeStemp());
                await _BadgerApiHelper.GenericPutAsyncString<string>(photoshoot.ToString(Formatting.None), "/Photoshoots/UpdatePhotoshootProductStatus/" + productId.ToString());
            }

            if (json.Value<string>("oldInternalNotes") != json.Value<string>("internalNotes"))
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
        Developer: Mohi
        Date: 8-21-19
        Request: GET
        Action:Get product with vendor for pair with autocomplete
        URL: product/PairWithProductsAutocomplete/id
        Input: product  id
        output: List of products with vendor 
        */
        [Authorize]
        [HttpGet("product/PairWithProductsAutocomplete/{id}")]
        public async Task<string> PairWithProductsAutocomplete(string id)
        {
            Object ProductsPairWithSearch = await _BadgerApiHelper.GenericGetAsync<Object>("/product/ProductsPairWithSearch/" + id);
            return ProductsPairWithSearch.ToString();
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
            var ProductList = await _BadgerApiHelper.GenericGetAsync<List<object>>("/product/getProductsbyVendor/" + vendor_id + "/" + productname);
            return JsonConvert.SerializeObject(ProductList);
        }

        /*
        Developer: Hamza Haq
        Date: 9-21-19
        Request: Post
        Action:create new/update fabric
        URL: /product/addFabric
        Input: fabric list and product name
        output: fabric id
        */
        [Authorize]
        [HttpPost("product/addFabric")]
        public async Task<string> addFabric([FromBody] List<Fabric> _fabrics)
        {

            string _userid = await _LoginHelper.GetLoginUserId();
            return await _ProductHelper.UpdateFabric(_fabrics, int.Parse(_userid));
        }
        /*
        Developer: Hamza Haq
        Date: 9-23-19
        Request: GEt
        Action:get products attribute (fabrics)
        URL: /product/getFabric/{productid}
        Input: (attribute) fabric list by productID
        output: fabric id
        */
        [Authorize]
        [HttpGet("product/getFabric/{productid}")]
        public async Task<object> getFabric(int productid)
        {

            return await _ProductHelper.GetFabric(productid);
        }
    }
}