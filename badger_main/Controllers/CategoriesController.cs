using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using badgerApi.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesRepository _CategoriesRepo;
        ILoggerFactory _loggerFactory;

        public CategoriesController(ICategoriesRepository CategoriesRepo, ILoggerFactory loggerFactory)
        {
            _CategoriesRepo = CategoriesRepo;
            _loggerFactory = loggerFactory;
        }

        /*
         Developer: Hamza Haq
         Date:8-23-19
         Action:Get List of categories data
         Request: GET
         URL: api/categories/list
         Input: /list
         output: list of attributes data
        */
        [HttpGet("list")]
        public ActionResult<List<Categories>> Get()
        {
            List<Categories> ToReturn = new List<Categories>();
            try
            {
                return _CategoriesRepo.allCategories;
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }

        }

        /*
          Developer: Hamza Haq
          Date:8-23-19
          Action:get HTML Form (New Styles Categories Data) from VIEW and pass the data to API Categories Repo 
          URL: /csategories/create
          Input: HTML form Body Json with the data of new Styles attributes and product_id
          output: New attribute id
          */
        // POST: api/attributes/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                Categories newCategories = JsonConvert.DeserializeObject<Categories>(value);
                NewInsertionID = await _CategoriesRepo.Create(newCategories);
            }
            catch(Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new attribute with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
         Developer: Hamza Haq
         Date:8-23-19
         Action:updating attrbutes data by id
         Request:PUT
         URL: api/categories/update/id
         Input: FormBody data and id
         output: Success/Failed
        */
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                Categories CategoriesToUpdate = JsonConvert.DeserializeObject<Categories>(value);
                CategoriesToUpdate.category_id = id;
                UpdateProcessOutput = await _CategoriesRepo.Update(CategoriesToUpdate);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  attribute with message" + ex.Message);
                UpdateResult = "Failed";
            }
            if (!UpdateProcessOutput)
            {
                UpdateResult = "Creation failed due to reason: No specific reson";
            }
            return UpdateResult;
        }
      

    }
}
