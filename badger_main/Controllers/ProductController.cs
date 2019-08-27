using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using GenericModals.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using badgerApi.Helper;
using System.Dynamic;
using itemService_entity.Models;
using GenericModals.PurchaseOrder;

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
        public IProductCategoriesRepository _ProductCategoriesRepository;
        public ProductController(IProductRepository ProductRepo, ILoggerFactory loggerFactory, INotesAndDocHelper notesAndDocHelper, IItemServiceHelper ItemsHelper, IProductCategoriesRepository ProductCategoriesRepository)
        {
            _ItemsHelper = ItemsHelper;
            _ProductRepo = ProductRepo;
            _loggerFactory = loggerFactory;
            _notesAndDocHelper = notesAndDocHelper;
            _ProductCategoriesRepository = ProductCategoriesRepository;
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
                productDetailsPageData.Product = await _ProductRepo.GetByIdAsync(Convert.ToInt32(id));
                productDetailsPageData.productProperties = await _ProductRepo.GetProductProperties(id);
                productDetailsPageData.productcolorwiths = await _ProductRepo.GetProductcolorwiths(id);
                productDetailsPageData.productpairwiths = await _ProductRepo.GetProductpairwiths(id);
                productDetailsPageData.product_Images = await _ProductRepo.GetProductImages(id);
                productDetailsPageData.ProductDetails = await _ProductRepo.GetProductDetails(id);
                List<Notes> note = await _notesAndDocHelper.GenericNote<Notes>(Convert.ToInt32(id), 1, 1);
                if (note.Count > 0)
                {
                    productDetailsPageData.Product_Notes = note[0].note;
                }
                else
                {
                    productDetailsPageData.Product_Notes = "";
                }

                productDetailsPageData.AllColors = await _ProductRepo.GetAllProductColors();
                productDetailsPageData.AllTags = await _ProductRepo.GetAllProductTags();
                productDetailsPageData.shootstatus = await _ProductRepo.GetProductShootStatus(id);
            }
            catch (Exception ex)
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
        [HttpPost("createitems/{qty}")]
        public async Task<string> PostItemsAsync([FromBody]   string value, int qty)
        {
            string NewInsertionID = "0";
            try
            {

                NewInsertionID = await _ItemsHelper.GenericPostAsync<String>(value.ToString(), "/item/create/" + qty.ToString());
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
                if (ProductToUpdate.product_name != "")
                {
                    ValuesToUpdate.Add("product_name", ProductToUpdate.product_name.ToString());
                }
                if (ProductToUpdate.vendor_color_name != "")
                {
                    ValuesToUpdate.Add("vendor_color_name", ProductToUpdate.vendor_color_name.ToString());
                }
                if (ProductToUpdate.product_cost != 0)
                {
                    ValuesToUpdate.Add("product_cost", ProductToUpdate.product_cost.ToString());
                }
                if (ProductToUpdate.product_retail != 0)
                {
                    ValuesToUpdate.Add("product_retail", ProductToUpdate.product_retail.ToString());
                }

                if (ProductToUpdate.product_vendor_image != null)
                {
                    ValuesToUpdate.Add("product_vendor_image", ProductToUpdate.product_vendor_image.ToString());
                }
                if (ProductToUpdate.sku_family != string.Empty)
                {
                    ValuesToUpdate.Add("sku_family", ProductToUpdate.sku_family.ToString());
                }
                if (ProductToUpdate.wash_type_id != 0)
                {

                    ValuesToUpdate.Add("wash_type_id", ProductToUpdate.wash_type_id.ToString());
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
                NewInsertionID = await _ProductRepo.CreatePOLineitems(newPOlineitems);
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
        // POST: api/product/createUsedIn   
        [HttpPost("createUsedIn")]
        public async Task<string> PostAsyncUsedIn([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                ProductUsedIn newProductUsedIn = JsonConvert.DeserializeObject<ProductUsedIn>(value);
                NewInsertionID = await _ProductRepo.CreateProductUsedIn(newProductUsedIn);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new line items with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
        Developer: Azeem hassan
        Date:28-7-19
        Action: sending data to db to insert image data
        URL: /product/createProductImage
        Input: FromBody string value
        output: new insertion id
        */
        // POST: api/product/createProductImage   
        [HttpPost("createProductImage")]
        public async Task<string> createProductImage([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                Productimages newProductImage = JsonConvert.DeserializeObject<Productimages>(value);
                NewInsertionID = await _ProductRepo.CreateProductImages(newProductImage);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new line items with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
        Developer: Azeem hassan
        Date:28-7-19
        Action: sending data to db to update image as primary
        URL: /product/updateProductImagePrimary
        Input: FromBody string value
        output: boolean
        */
        // POST: api/product/updateProductImagePrimary   
        [HttpPost("updateProductImagePrimary")]
        public async Task<Boolean> updateProductImagePrimary([FromBody]   string value)
        {
            Boolean updateResult = false;
            try
            {
                dynamic dataImage = JsonConvert.DeserializeObject<Object>(value);
                int product_img_id = dataImage.product_img_id;
                int is_primary = dataImage.is_primary;

                updateResult = await _ProductRepo.UpadateImagePrimary(product_img_id, is_primary);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new line items with message" + ex.Message);
            }
            return updateResult;
        }

        /*
        Developer: Hamza Haq
        Date:23-8-19
        Action: 
        URL: /product/createProductCategory
        Input: FromBody string value
        output: boolean
        */
        //POST: api/product/CreateProductCategory
        [HttpPost("UpdateProductCategory")]
        public async Task<string> UpdateProductCategory([FromBody]  string value)
        {
            string updateResult = "";
            try
            {
                ProductCategories productCategories = JsonConvert.DeserializeObject<ProductCategories>(value);
                if (productCategories.action.ToLower() == "insert")
                {
                    updateResult = await _ProductCategoriesRepository.Create(productCategories);
                }
                else
                {
                    updateResult = await _ProductCategoriesRepository.Delete(productCategories);
                }

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new line items with message" + ex.Message);
            }
            return updateResult;
        }

        /*
        Developer: Sajid Khan
        Date: 08-09-19 
        Action: Getting list of product by product_name string
        URL:  api/product/getproduct/product_name
        Request GET
        Input: string product_name 
        output: dynamic list of products
        */
        [HttpGet("getproduct/{product_name}")]
        public async Task<List<object>> GetProduct(string product_name)
        {
            dynamic productDetails = new object();
            try
            {
                productDetails = await _ProductRepo.GetProduct(product_name);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return productDetails;

        }

        /*
        Developer: Sajid Khan
        Date: 24-8-19 
        Action: Get mutiple product ids with comma seperate  "api/purchaseorders/getproductidsbypurchaseorder"
        URL: api/purchaseorders/getproductidsbypurchaseorder
        Request: Get
        Input: string poids
        output: list of mutiple product ids
        */
        [HttpGet("getproductidsbypurchaseorder")]
        public async Task<object> GetProductIdsByPurchaseOrder()
        {
            dynamic poPageList = new object();
            try
            {
                poPageList = await _ProductRepo.GetProductIdsByPurchaseOrder();

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseorders GetProductIdsByPurchaseOrder with message" + ex.Message);

            }

            return poPageList;

        }

        /*
        Developer: Rizwan Ali
        Date: 24-8-19 
        Action: Get published product ids  "api/purchaseorders/getpublishedproductCount/1,2,3"
        URL: api/purchaseorders/getpublishedproductCount/1,2,3
        Request: Get
        Input: string productids
        output: list of published product ids
        */
        [HttpGet("getpublishedproductCount/{productids}")]
        public async Task<object> GetPublishedProductCount(string productids)
        {
            dynamic poPageList = new object();
            try
            {
                poPageList = await _ProductRepo.GetPublishedProductIds(productids);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseorders GetPublishedProductCount with message" + ex.Message);

            }

            return poPageList;

        }

        

        /*
       Developer: Rizvan Ali
       Date:5-7-19
       Action:get HTML Form (New Styles Data) from VIEW and pass the data to delete product
       URL: /product/create
       Input: Product ID
       output: status
       */
        // POST: api/product/delete
        [HttpGet("delete/{product_id}")]
        public async Task<bool> DelAsync(string product_id)
        {
            bool isDeleted = false;
            try
            {
                bool isItemDeleted = await _ItemsHelper.DeleteItemByProduct(product_id.ToString());
                isDeleted = await _ProductRepo.DeleteProduct(product_id);
                isDeleted = isItemDeleted && isDeleted;

            }
            catch (Exception ex)
            {
                isDeleted = false;
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in deleting product with message" + ex.Message);
            }
            return isDeleted;
        }


    }
}
