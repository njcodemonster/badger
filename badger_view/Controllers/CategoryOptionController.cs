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
    public class CategoryOptionController : Controller
    {
        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        private CommonHelper.awsS3helper awsS3Helper = new CommonHelper.awsS3helper();
        private String UploadPath = "";
        private String S3bucket = "";
        private String S3folder = "";
        private ILoginHelper _LoginHelper;
        public CategoryOptionController(IConfiguration config, ILoginHelper LoginHelper)
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
       Developer: Rizwan Ali
       Date: 8-25-19 
       Action:Get all tags from category option table for All Sub Category 
       URL: categoryoption/GetTagsSubCategoryWise/
       Input: none
       output:  List of All Tags and Categories
       */
        public async Task<IActionResult> Index()
        {
            CategoryOptionPage categoryOption = new CategoryOptionPage();
            SetBadgerHelper();
            ViewBag.SubCats = await _BadgerApiHelper.GenericGetAsync<IEnumerable<Categories>>("/CategoryOption/SubCategoryAll/");
            categoryOption = await _BadgerApiHelper.GenericGetAsync<CategoryOptionPage>("/CategoryOption/CategoryOptionPage/");
            ViewBag.selected = 0;
            return View(categoryOption);
        }
        /*
        Developer: Rizwan Ali
        Date: 8-25-19 
        Action:Get all tags from category option table for specific or All Sub Category 
        URL: categoryoption/GetTagsSubCategoryWise/
        Input: SubCategoryID
        output: List of All Tags (Specific SubCategory) and Categories
        */
        public async Task<IActionResult> GetTagsSubCategoryWise(string id)
        {
            CategoryOptionPage categoryOption = new CategoryOptionPage();
            SetBadgerHelper();
            if (id == "0")
            {
                ViewBag.SubCats = await _BadgerApiHelper.GenericGetAsync<IEnumerable<object>>("/CategoryOption/SubCategoryAll/");
                categoryOption = await _BadgerApiHelper.GenericGetAsync<dynamic>("/CategoryOption/CategoryOptionPage/");
                ViewBag.selected = 0;
            }
            else
            {
                ViewBag.SubCats = await _BadgerApiHelper.GenericGetAsync<IEnumerable<Categories>>("/CategoryOption/SubCategoryAll/");
                ViewBag.selected = Convert.ToInt32(id);
                categoryOption = await _BadgerApiHelper.GenericGetAsync<CategoryOptionPage>("/CategoryOption/CategoryOptionPageTypeWise/" + id);
                var arr = categoryOption.AllTags.Where(p => p.isChecked == "checked").Select(p => p.attribute_id).Distinct().ToList();
                ViewBag.tags = JsonConvert.SerializeObject(arr);
            }
            return PartialView("Index", categoryOption);
        }

        /*
        Developer: Rizwan Ali
        Date: 8-25-19 
        Action:Get all parent categories from db where parent category id=0 in categories table 
        URL: categoryoption/getParentCategory/
        Input: none
        output: List of categories to sow on a drop down 
        */
        [Authorize]
        [HttpGet("categoryoption/getParentCategory/")]
        public async Task<Object> GetParentCategory()
        {
            SetBadgerHelper();
            dynamic ParentCategory = new ExpandoObject();
            ParentCategory.vendorProducts = await _BadgerApiHelper.GenericGetAsync<object>("/CategoryOption/getParentCategory/");
            return JsonConvert.SerializeObject(ParentCategory);
        }


        /*
        Developer: Rizwan Ali
        Date:8-25-19 
        Action:get HTML Form data of new Parent CAtegory and pass the data to  API functions 
        URL: /Category/Create
        Input: HTML form with the data of new Parent Category Like Clothing Acccesories
        output: New cat id if 0 then error 
        */

        [HttpPost("/Category/Create")]
        public async Task<String> CreateNewCategory([FromBody]   JObject json)
        {
            SetBadgerHelper();
            string ParentCategoryID = string.Empty;
            // add new Parent Category in categories table
            JObject cat = new JObject();
            Int32 category_type = json.Value<Int32>("ParentCatId");
            string category_name = json.Value<string>("subCatTitle");
            Int32 category_parent_id = json.Value<Int32>("ParentCatId");
            cat.Add("category_type", category_type);
            cat.Add("category_name", category_name);
            cat.Add("category_parent_id", category_parent_id);
            ParentCategoryID = await _BadgerApiHelper.GenericPostAsyncString<String>(cat.ToString(Formatting.None), "/CategoryOption/createParentCategory");
          
            return ParentCategoryID;
        }

        /*
        Developer: Rizvan Ali
        Date:8-25-19 
        Action:get HTML Form from VIEW and pass the data to multiple API functions 
        URL: /product/create
        Input: HTML form Body Json with the data of new attributes/Option that are added or deleted
        output: Status
        */
        // POST: api/CategoryOption/UpdateAttributes
        [Authorize]
        [HttpPost("categoryoption/updateattributes/")]
        public async Task<string> UpdateAttributes([FromBody]   JObject json)
        {
            SetBadgerHelper();
            int category_id = json.Value<Int32>("category_id");
            JArray added= json.Value<JArray>("tag_added");
            JArray newAdded = new JArray();
            List<CategoryOptions> categories = new List<CategoryOptions>();
            foreach (var item in added)
            {
                JObject cat = new JObject();
                cat.Add("attribute_id" , Convert.ToInt32(item.Value<Int32>()));
                cat.Add("category_id", category_id);
                cat.Add("created_at",_common.GetTimeStemp());
                newAdded.Add(cat);
            }
            string insert_status = "True";
            if (added.Count > 0)
            {
                 insert_status = await _BadgerApiHelper.GenericPostAsyncString<string>(newAdded.ToString(Formatting.None), "/CategoryOption/createCategoryOption");
            }
            JArray removed = json.Value<JArray>("tag_removed");
            JArray newRemoved = new JArray();
            foreach (var item in removed)
            {
                JObject cat = new JObject();
                cat.Add("attribute_id", Convert.ToInt32(item.Value<Int32>()));
                cat.Add("category_id", category_id);
                newRemoved.Add(cat);
            }
            string delete_status = "True";
            if (newRemoved.Count>0)
            {
                delete_status = await _BadgerApiHelper.GenericPostAsyncString<string>(newRemoved.ToString(Formatting.None), "/CategoryOption/deleteCategoryOption");
            }
            if(insert_status.ToLower() == "true" && delete_status.ToLower() == "true")
                 return "true";
            else
                return "false";
        }

    }
}