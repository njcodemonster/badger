using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using badgerApi.Helper;
using badgerApi.Interfaces;
using GenericModals.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryOptionController : Controller
    {
        private readonly ICategoryRepository _CategoryRepo;
        private IItemServiceHelper _ItemsHelper;
        ILoggerFactory _loggerFactory;
        INotesAndDocHelper _notesAndDocHelper;
        public CategoryOptionController(ICategoryRepository CategoryRepo, ILoggerFactory loggerFactory, INotesAndDocHelper notesAndDocHelper, IItemServiceHelper ItemsHelper)
        {
            _ItemsHelper = ItemsHelper;
            _CategoryRepo = CategoryRepo;
            _loggerFactory = loggerFactory;
            _notesAndDocHelper = notesAndDocHelper;
        }


        /*
        Developer: Rizwan Ali
        Date:8-23-19 
        Action: getting CategoryOption Like tags and color All
        Request:GET
        URL: api/Product/CategoryOptionPageTypeWise
        Input: none
        output: CategoryOptionPage
        */
        [HttpGet("CategoryOptionPage")]
        public async Task<CategoryOptionPage> GetCategoryOption()
        {
            CategoryOptionPage categoryOption = new CategoryOptionPage();
            try
            {
                //categoryOption.AllColors = await _CategoryRepo.GetAllColors();
                categoryOption.AllTags = await _CategoryRepo.GetAllTags();
            }
            catch (Exception ex)
            {

                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in getting data for selecting category option with message" + ex.Message);
            }
            return categoryOption;
        }

        /*
       Developer: Rizwan Ali
       Date:8-23-19
       Action: getting All sub Categories where parent id not equals 0
       Request:GET
       URL: api/CategoryOption/SubCategoryAll
       Input: none
       output: CategoryOptionPage
        */
        [HttpGet("SubCategoryAll")]
        public async Task<IEnumerable<Categories>> GetSubCategoryAll()
        {
            IEnumerable<Categories> categoryOption = new List<Categories>();
            try
            {
                categoryOption = await _CategoryRepo.GetSubCategoryAll();
            }
            catch (Exception ex)
            {

                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in getting sub category with message" + ex.Message);
            }
            return categoryOption;
        }
        /*
       Developer: Rizwan Ali
       Date:8-23-19 
       Action: getting CategoryOption Like tags by Sending specific Category ID
       Request:GET
       URL: api/CategoryOption/CategoryOptionPageTypeWise/{id}
       Input: Category id
       output: CategoryOptionPage
       */
        [HttpGet("CategoryOptionPageTypeWise/{id}")]
        public async Task<CategoryOptionPage> GetCategoryOptionTypeWise(string id)
        {
            CategoryOptionPage categoryOption = new CategoryOptionPage();
            try
            {
                //categoryOption.AllColors = await _CategoryRepo.GetAllColors();
                categoryOption.AllTags = await _CategoryRepo.GetAllTagsTypeWise(id);
            }
            catch (Exception ex)
            {

                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in getting data for selecting category option with message" + ex.Message);
            }
            return categoryOption;
        }


        /*
        Developer: Rizwan Ali
        Date:8-23-19
        Action: Get Parent Category where Parent id =0 from table categories
        Request:GET
        URL: api/CategoryOption/GetParentCategory
        Input: none
        output: CategoryOption
        */
        [HttpGet("getParentCategory")]
        public async Task<object> GetParentCategory()
        {
            dynamic categoryOption = new ExpandoObject();
            try
            {
                categoryOption = await _CategoryRepo.GetParentCategory();
            }
            catch (Exception ex)
            {

                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in getting sub category with message" + ex.Message);
            }
            return categoryOption;
        }

        /*
        Developer: Rizvan Ali
        Date:8-23-19
        Action:get HTML Form  from VIEW and pass the data to API Category Repo 
        URL: api/CategoryOption/createParentCategory/
        Input: HTML form Body Json with the data of new Category
        output: New Catgory id
        */
        // POST: api/CategoryOption/createParentCategory
        [HttpPost("createParentCategory")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                Categories newCategory = JsonConvert.DeserializeObject<Categories>(value);
                NewInsertionID = await _CategoryRepo.Create(newCategory);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Parent Category with message" + ex.Message);
            }
            return NewInsertionID;
        }
        /*
        Developer: Rizvan Ali
        Date:8-23-19
        Action:get HTML Form from VIEW and pass the data to API Category Repo 
        URL: /CategoryOption/createCategoryOption
        Input: HTML form Body Json with the data of new Category Option 
        output: Status
        */
        // POST: api/CategoryOption/createCategoryOption
        [HttpPost("createCategoryOption")]
        public async Task<string> CreateCategoryOption([FromBody]   string value)
        {
            bool status =false;
            try
            {
                List<CategoryOptions> newCategory = JsonConvert.DeserializeObject<List<CategoryOptions>>(value);
                var a = await _CategoryRepo.CreateCategoryOption(newCategory);
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Category option with message" + ex.Message);
            }
            return status.ToString() ;
        }
        /*
         Developer: Rizvan Ali
         Date:8-23-19
         Action:get HTML Form (New Styles Data) from VIEW and pass the data to API Category Repo 
         URL: /CategoryOption/deleteCategoryOption
         Input: HTML form Body Json with the data of new product
         output: New product id
         */
        // POST: api/CategoryOption/deleteCategoryOption
        [HttpPost("deleteCategoryOption")]
        public async Task<string> DeleteCategoryOption([FromBody]   string value)
        {
            bool status = false;
            try
            {
                List<CategoryOptions> newCategory = JsonConvert.DeserializeObject<List<CategoryOptions>>(value);
                var a = await _CategoryRepo.DeleteCategoryOption(newCategory);
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in deleting Category option with message" + ex.Message);
            }
            return status.ToString();
        }
    }
}