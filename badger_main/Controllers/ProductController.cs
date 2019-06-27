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
using System.Dynamic;
using badgerApi.Helper;
using itemService_entity.Models;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly IProductRepository _ProductRepo;
        private IItemServiceHelper _ItemsHelper;
        ILoggerFactory _loggerFactory;
        public ProductController(IProductRepository ProductRepo, ILoggerFactory loggerFactory, IItemServiceHelper ItemsHelper)
        {
            _ItemsHelper = ItemsHelper;
            _ProductRepo = ProductRepo;
            _loggerFactory = loggerFactory;
        }

        // GET: api/Product
        [HttpGet("list")]
        public async Task<ActionResult<List<Product>>> GetAsync()
        {
            List<Product> ToReturn = new List<Product>();
            try
            {
                return await _ProductRepo.GetAll(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }


        }
        // GET: api/Product/detailpage/1
        [HttpGet("detailpage/{id}")]
        public async Task<ProductDetailsPageData> GetProductDetailPage(string id)
        {
            ProductDetailsPageData productDetailsPageData = new ProductDetailsPageData();
            productDetailsPageData.productProperties = await _ProductRepo.GetProductProperties(id);
            
            return productDetailsPageData;

        }
        // GET: api/Product/list/1
        [HttpGet("list/{id}")]
        public async Task<List<Product>> GetAsync(int id)
        {
            List<Product> ToReturn = new List<Product>();
            try
            {
                Product Res = await _ProductRepo.GetByIdAsync(id);
                ToReturn.Add(Res);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return ToReturn;
        }




        // POST: api/product/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                Product newProduct = JsonConvert.DeserializeObject<Product>(value);
                NewInsertionID = await _ProductRepo.Create(newProduct);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new product with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // POST: api/product/create/items
        [HttpPost("create/items")]
        public async Task<string> PostItemsAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                
                NewInsertionID = await _ItemsHelper.GenericPostAsync<String>(value.ToString(), "/item/create");
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new product with message" + ex.Message);
            }
            return NewInsertionID;
        }


        // PUT: api/product/updatespecific/5
        [HttpPut("updatespecific/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";

            try
            {
                Product ProductToUpdate = JsonConvert.DeserializeObject<Product>(value);
                ProductToUpdate.product_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
                if (ProductToUpdate.product_vendor_image != "")
                {
                    ValuesToUpdate.Add("product_vendor_image", ProductToUpdate.product_vendor_image.ToString());
                }
                
                await _ProductRepo.UpdateSpecific(ValuesToUpdate, "Product_id=" + id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new Product with message" + ex.Message);
                UpdateResult = "Failed";
            }

            return UpdateResult;
        }


        // POST: api/product/create
        [HttpPost("createProductAttribute")]
        public async Task<string> PostAsyncAttribute([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                ProductAttributes newProductAttributes = JsonConvert.DeserializeObject<ProductAttributes>(value);
                NewInsertionID = await _ProductRepo.CreateProductAttribute(newProductAttributes);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new product with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // POST: api/product/create
        [HttpPost("createAttributesValues")]
        public async Task<string> PostAsyncAttributesValues([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                ProductAttributeValues newProductAttributeValues = JsonConvert.DeserializeObject<ProductAttributeValues>(value);
                NewInsertionID = await _ProductRepo.CreateAttributeValues(newProductAttributeValues);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new product with message" + ex.Message);
            }
            return NewInsertionID;
        }
        // POST: api/product/create
        [HttpPost("createSku")]
        public async Task<string> PostAsyncSku([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                Sku newSku = JsonConvert.DeserializeObject<Sku>(value);
                NewInsertionID = await _ProductRepo.CreateSku(newSku);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Sku with message" + ex.Message);
            }
            return NewInsertionID;
        }
        // POST: api/product/create
        [HttpPost("createLineitems")]
        public async Task<string> PostAsyncLineitem([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrderLineItems newPOlineitems = JsonConvert.DeserializeObject<PurchaseOrderLineItems>(value);
                NewInsertionID = await _ProductRepo.CreatePOLineitems (newPOlineitems);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new line items with message" + ex.Message);
            }
            return NewInsertionID;
        }
    }
}
