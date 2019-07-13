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
        public ProductController(IProductRepository ProductRepo, ILoggerFactory loggerFactory,INotesAndDocHelper notesAndDocHelper , IItemServiceHelper ItemsHelper)
        {
            _ItemsHelper = ItemsHelper;
            _ProductRepo = ProductRepo;
            _loggerFactory = loggerFactory;
            _notesAndDocHelper = notesAndDocHelper;
        }

        // GET: api/Product
        /*
         Developer: Azeem Hassan
         Date:7-8-19
         Action: getting all Products list
         Request:GET
         URL: api/Product
         Input: null
         output: list of Products
      */
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
        /*
           Developer: Azeem Hassan
           Date:7-8-19
           Action: getting ProductDetail by id
           Request:GET
           URL: api/Product/detailpage/1
           Input: int id
           output: ProductDetails
        */
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

        /*
           Developer: Azeem Hassan
           Date:7-8-19
           Action: getting product by id
           Request:GET
           URL: api/Product/list/1
           Input: int id
           output: product
        */
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



        /*
        Developer: ubaid
        Date:5-7-19
        Action:get HTML Form (New Styles Data) from VIEW and pass the data to API product Repo 
        URL: /product/create
        Input: HTML form Body Json with the data of new product
        output: New product id
        */
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

        /*
       Developer: ubaid
       Date:5-7-19
       Action:get HTML Form (New Styles Data) from VIEW and pass the data to items API
       URL: /product/createitems/{quantity}
       Input: HTML form Body Json with the data of new product
       output: New item id
       */
        // POST: api/product/create/items
        /*
          Developer: Azeem Hassan
          Date:7-8-19
          Action: Inserting new items
          Request:POST
          URL:  api/product/create/items
          Input: FormBody data and quantity
          output: NewInsertionID
       */
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

        /*
         Developer: Azeem Hassan
         Date:7-8-19
         Action: Update specific product by id
         Request:PUT
         URL:  api/product/updatespecific/5
         Input: FormBody data and int id
         output: Success/Failed
      */
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

        /*
            Developer: Azeem Hassan
            Date:7-8-19
            Action: Update specific product attribute by id
            Request:PUT
            URL:  api/Product/attribute/updatespecific/5
            Input: FormBody data and int id
            output: Success/Failed
        */


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



        /*
        Developer: ubaid
        Date:5-7-19
        Action:get HTML Form (New Attribute Data) from VIEW and pass the data to API product Repo 
        URL: /product/createProductAttribute
        Input: HTML form Body Json with the data of new Attribute and product_id
        output: New Attribute id
        */
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

        /*
       Developer: ubaid
       Date:5-7-19
       Action:get HTML Form (New Styles Attributes Values Data) from VIEW and pass the data to API product Repo 
       URL: /product/createAttributesValues
       Input: HTML form Body Json with the data of new Attribute values and product_id and new Attribute_id
       output: New Attribute value id
       */
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

        /*
       Developer: ubaid
       Date:5-7-19
       Action:get HTML Form (New Styles SKU Data) from VIEW and pass the data to API product Repo 
       URL: /product/createSku
       Input: HTML form Body Json with the data of new SKU values and product_id
       output: New SKU id
       */
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

        /*
          Developer: ubaid
          Date:5-7-19
          Action:get HTML Form (New Styles LINE ITEMS Data) from VIEW and pass the data to API product Repo 
          URL: /product/createLineitems
          Input: HTML form Body Json with the data of new LINE ITEMS values and product_id
          output: New LINE ITEM id
          */
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
        /*
          Developer: ubaid
          Date:13-7-19
          Action:get HTML Form (POid and product id) from VIEW and pass the data to API product Repo 
          URL: /product/createUsedIn
          Input: HTML form Body Json with the data of new LINE ITEMS values and product_id
          output: New LINE ITEM id
          */
        // POST: api/product/createUsedIn   // purchase order line items
        [HttpPost("createUsedIn")]
        public async Task<string> PostAsyncUsedIn([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrderLineItems newPOlineitems = JsonConvert.DeserializeObject<PurchaseOrderLineItems>(value);
                NewInsertionID = await _ProductRepo.CreatePOLineitems(newPOlineitems);
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
