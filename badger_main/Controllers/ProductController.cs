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
using badgerApi.Helper;
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
        INotesAndDocHelper _notesAndDocHelper;
        public ProductController(IProductRepository ProductRepo, ILoggerFactory loggerFactory,INotesAndDocHelper notesAndDocHelper)
        {
            _ItemsHelper = ItemsHelper;
            _ProductRepo = ProductRepo;
            _loggerFactory = loggerFactory;
            _notesAndDocHelper = notesAndDocHelper;
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
            try
            {
                productDetailsPageData.Product = await _ProductRepo.GetByIdAsync(Convert.ToInt32( id));
                productDetailsPageData.productProperties = await _ProductRepo.GetProductProperties(id);
                productDetailsPageData.productcolorwiths = await _ProductRepo.GetProductcolorwiths(id);
                productDetailsPageData.productpairwiths = await _ProductRepo.GetProductpairwiths(id);
                productDetailsPageData.product_Images = await _ProductRepo.GetProductImages(id);
                productDetailsPageData.ProductDetails = await _ProductRepo.GetProductDetails(id);
                List<Notes> note = await _notesAndDocHelper.GenericNote<Notes>(Convert.ToInt32(id), 1, 1);
                if (note.Count > 0) {
                    productDetailsPageData.Product_Notes = note[0].note;
                }
                else
                {
                    productDetailsPageData.Product_Notes = "";
                }
                
                productDetailsPageData.AllColors = await _ProductRepo.GetAllProductColors();
                productDetailsPageData.AllTags = await _ProductRepo.GetAllProductTags();
                productDetailsPageData.shootstatus = await _ProductRepo.GetProductShootStatus(id);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
        [HttpPost("createitems/{qty}")]
        public async Task<string> PostItemsAsync([FromBody]   string value,int qty)
        {
            string NewInsertionID = "0";
            try
            {
                
                NewInsertionID = await _ItemsHelper.GenericPostAsync<String>(value.ToString(), "/item/create/"+ qty.ToString());
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
                if (ProductToUpdate.product_vendor_image != null)
                {
                    ValuesToUpdate.Add("product_vendor_image", ProductToUpdate.product_vendor_image.ToString());
                }
                if (ProductToUpdate.sku_family != null)
                {
                    ValuesToUpdate.Add("sku_family", ProductToUpdate.sku_family.ToString());
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


        // PUT: api/Product/attribute/updatespecific/5
        [HttpPut("attribute/updatespecific/{id}")]
        public async Task<string> AttributeUpdateSpecific(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";

            try
            {
                ProductAttributes ProductAttributeToUpdate = JsonConvert.DeserializeObject<ProductAttributes>(value);
                ProductAttributeToUpdate.product_attribute_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
                
                if (ProductAttributeToUpdate.sku != null)
                {
                    ValuesToUpdate.Add("sku", ProductAttributeToUpdate.sku.ToString());
                }

                await _ProductRepo.AttributeUpdateSpecific(ValuesToUpdate, "product_attribute_id=" + id);
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
        // POST: api/product/create   // purchase order line items
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
