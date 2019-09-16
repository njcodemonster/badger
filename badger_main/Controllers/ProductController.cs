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
        private int note_type = 1;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        private INotesAndDocHelper _NotesAndDoc;
        private IEventRepo _eventRepo;


        string event_product_note_create = "Product note created by user = %%userid%% with product id = %%pid%%"; ///event_create_photoshoot_note
        int event_product_note_create_id = 37;
        string table_name = "product_events";
        string user_event_table_name = "user_events";

        public ProductController(IProductRepository ProductRepo, ILoggerFactory loggerFactory,INotesAndDocHelper notesAndDocHelper , IItemServiceHelper ItemsHelper, INotesAndDocHelper NotesAndDoc, IEventRepo eventRepo, IProductCategoriesRepository ProductCategoriesRepository)
        {
            _ItemsHelper = ItemsHelper;
            _ProductRepo = ProductRepo;
            _loggerFactory = loggerFactory;
            _notesAndDocHelper = notesAndDocHelper;
            _NotesAndDoc = NotesAndDoc;
            _eventRepo = eventRepo;
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
        Developer: Mohi
        Date:8-21-19
        Action: getting all Products list with vendor for Product Pair With 
        Request:GET
        URL: api/Product
        Input: null
        output: list of Products
        */
        [HttpGet("ProductsPairWithSearch/{prodcut_id}")]
        public async Task<object> ProductsPairWithSearch(string prodcut_id)
        {
            dynamic ToReturn = new object();
            try
            {
                ToReturn = await _ProductRepo.GetProductsPairWithSearch(prodcut_id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for ProductsPairWithSearch with message" + ex.Message);
            }
            return ToReturn;
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
                productDetailsPageData.shootModels= await _ProductRepo.GetPhotoshootModels();
                productDetailsPageData.productPhotoshootModel = await _ProductRepo.GetProductPhotoshootModel(id);
            }
            catch(Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for ProductDetailsPageData with message" + ex.Message);
            }
            return productDetailsPageData;

        }

        /*
           Developer: Mohi
           Date:8-8-19
           Action: Update product Attributes by product id
           Request:PUT
           URL:  api/product/UpdateAttributes/5
           Input: FormBody data and int id
           output: Success/Failed
        */
        [HttpPut("UpdateAttributes/{id}")]
        public async Task<string> UpdateAttributes(int id, [FromBody] string value)
        {
            string UpdateResult = "Success";
            try
            {
                dynamic ProductToUpdate = JsonConvert.DeserializeObject<dynamic>(value);
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
                if (ProductToUpdate.product_name != null)
                {
                    ValuesToUpdate.Add("product_name", ProductToUpdate.product_name.ToString());
                }
                if (ProductToUpdate.size_and_fit_id != null)
                {
                    ValuesToUpdate.Add("size_and_fit_id", ProductToUpdate.size_and_fit_id.ToString());
                }
                if (ProductToUpdate.product_retail != null)
                {
                    ValuesToUpdate.Add("product_retail", ProductToUpdate.product_retail.ToString());
                }
                if (ProductToUpdate.product_cost != null)
                {
                    ValuesToUpdate.Add("product_cost", ProductToUpdate.product_cost.ToString());
                }
                if (ProductToUpdate.product_discount != null)
                {
                    ValuesToUpdate.Add("product_discount", ProductToUpdate.product_discount.ToString());
                }
                if (ProductToUpdate.updated_by != null)
                {
                    ValuesToUpdate.Add("updated_by", ProductToUpdate.updated_by.ToString());
                }
                if (ProductToUpdate.updated_at != null)
                {
                    ValuesToUpdate.Add("updated_at", ProductToUpdate.updated_at.ToString());
                }  

                await _ProductRepo.UpdateSpecific(ValuesToUpdate, "product_id=" + id);

                Dictionary<String, String> ProductDetailValuesToUpdate = new Dictionary<string, string>();
                if (ProductToUpdate.product_detail_1 != null  && ProductToUpdate.product_detail_1.ToString() != "")
                {
                    await _ProductRepo.AddEditProductPageDetails(id.ToString(), "1", ProductToUpdate.product_detail_1.ToString(), ProductToUpdate.updated_by.ToString(), ProductToUpdate.updated_at.ToString());
                }
                if (ProductToUpdate.product_detail_2 != null && ProductToUpdate.product_detail_2.ToString() != "")
                {
                    await _ProductRepo.AddEditProductPageDetails(id.ToString(), "2", ProductToUpdate.product_detail_2.ToString(), ProductToUpdate.updated_by.ToString(), ProductToUpdate.updated_at.ToString());
                }
                if (ProductToUpdate.product_detail_3 != null && ProductToUpdate.product_detail_3.ToString() != "")
                {
                    await _ProductRepo.AddEditProductPageDetails(id.ToString(), "3", ProductToUpdate.product_detail_3.ToString(), ProductToUpdate.updated_by.ToString(), ProductToUpdate.updated_at.ToString());
                }
                if (ProductToUpdate.product_detail_4 != null && ProductToUpdate.product_detail_4.ToString() != "")
                {
                    await _ProductRepo.AddEditProductPageDetails(id.ToString(), "4", ProductToUpdate.product_detail_4.ToString(), ProductToUpdate.updated_by.ToString(), ProductToUpdate.updated_at.ToString());
                }


                if (ProductToUpdate.pairProductIds != null && ProductToUpdate.pairProductIds.ToString() != "")
                {
                    string PairProductIds = ProductToUpdate.pairProductIds.ToString();
                    int countComma = PairProductIds.Count(c => c == ',');
                    if (countComma > 0)
                    {
                        var ids = PairProductIds.Split(",");
                        foreach (var pair_product_id in ids)
                        {
                            await _ProductRepo.AddPairWithProduct(id.ToString(), pair_product_id, ProductToUpdate.updated_by.ToString(), ProductToUpdate.updated_at.ToString());
                        }
                    }
                    else
                    {
                        await _ProductRepo.AddPairWithProduct(id.ToString(), PairProductIds, ProductToUpdate.updated_by.ToString(), ProductToUpdate.updated_at.ToString());
                    }
                }

                if (ProductToUpdate.RemovePairWithProductIds != null && ProductToUpdate.RemovePairWithProductIds.ToString() != "")
                {
                    string RemovePairProductIds = ProductToUpdate.RemovePairWithProductIds.ToString();
                    int countComma = RemovePairProductIds.Count(c => c == ',');
                    if (countComma > 0)
                    {
                        var ids = RemovePairProductIds.Split(",");
                        foreach (var pair_product_id in ids)
                        {
                            await _ProductRepo.RemovePairWithProduct(id.ToString(), pair_product_id);
                        }
                    }
                    else
                    {
                        await _ProductRepo.RemovePairWithProduct(id.ToString(), RemovePairProductIds);
                    }
                }

                if (ProductToUpdate.RemoveOtherColorProductIds != null && ProductToUpdate.RemoveOtherColorProductIds.ToString() != "")
                {
                    string RemoveOtherColorProductIds = ProductToUpdate.RemoveOtherColorProductIds.ToString();
                    int countComma = RemoveOtherColorProductIds.Count(c => c == ',');
                    if (countComma > 0)
                    {
                        var ids = RemoveOtherColorProductIds.Split(",");
                        foreach (var pair_product_id in ids)
                        {
                            await _ProductRepo.RemoveOtherColorProducts(id.ToString(), pair_product_id);
                        }
                    }
                    else
                    {
                        await _ProductRepo.RemoveOtherColorProducts(id.ToString(), RemoveOtherColorProductIds);
                    }
                }


                if (ProductToUpdate.otherColorsProductIds != null && ProductToUpdate.otherColorsProductIds.ToString() != "")
                {
                    string OtherColorsProductIds = ProductToUpdate.otherColorsProductIds.ToString();
                    int countComma = OtherColorsProductIds.Count(c => c == ',');
                    if (countComma > 0)
                    {
                        var ids = OtherColorsProductIds.Split(",");
                        foreach (var pair_product_id in ids)
                        {
                            await _ProductRepo.AddOtherColorProducts(id.ToString(), pair_product_id, ProductToUpdate.updated_by.ToString(), ProductToUpdate.updated_at.ToString());
                        }
                    }
                    else
                    {
                        await _ProductRepo.AddOtherColorProducts(id.ToString(), OtherColorsProductIds, ProductToUpdate.updated_by.ToString(), ProductToUpdate.updated_at.ToString());
                    }
                }

                /**
                                product.Add("", json.Value<string>("product_detail_1"));
                                product.Add("product_detail_2", json.Value<string>("product_detail_2"));
                                product.Add("product_detail_3", json.Value<string>("product_detail_3"));
                                product.Add("product_detail_4", json.Value<string>("product_detail_4"));*/

                //UpdateSpecificData
            }
            catch (Exception ex)
            {
                UpdateResult = "Failed";
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating product attribute with message" + ex.Message);
            }
            return UpdateResult;
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
        Developer: Hamza Haq
        Date:30-8-19
        Action:get HTML Form (Update Styles Data) from VIEW and pass the data to items API
        URL: /product/updateitems/{quantity}
        Input: HTML form Body Json with the data of new product
        output: old item id
       */
        [HttpPost("updateitems/{qty}")]
        public async Task<string> updateItemsAsync([FromBody]  string value, int qty)
        {
            string NewInsertionID = "0";
            try
            {

                NewInsertionID = await _ItemsHelper.GenericPostAsync<String>(value.ToString(), "/item/update/" + qty.ToString());
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new product with message" + ex.Message);
            }
            return NewInsertionID;
        }
        /*
         Developer: Hamza Haq
         Date:31-8-19
         Action:get Item Count by status - (not received)
         URL: /product/getitems/{po_id}/{sku}
         Input: Po ID and Sku
         output: old item id
        */
        [HttpGet("getitems/{po_id}/{sku}")]
        public async Task<string> getitemsAsync(int po_id, string sku)
        {
            string itemQuantity = "0";
            try
            {
                itemQuantity = await _ItemsHelper.GenericGetsAsync("/item/GetitemCountBySkuStatus/" + po_id.ToString() + "/" + sku + "/1");
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new product with message" + ex.Message);
            }
            return itemQuantity;
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
                if (ProductToUpdate.product_name != null)
                {
                    ValuesToUpdate.Add("product_name", ProductToUpdate.product_name.ToString());
                }
                if (ProductToUpdate.vendor_color_name != null)
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
                if (ProductToUpdate.sku_family != null)
                {
                    ValuesToUpdate.Add("sku_family", ProductToUpdate.sku_family.ToString());
                }
                if (ProductToUpdate.wash_type_id == 0 || ProductToUpdate.wash_type_id > 0) 
                {
                    ValuesToUpdate.Add("wash_type_id", ProductToUpdate.wash_type_id.ToString());
                }
                if (ProductToUpdate.updated_by != 0)
                {
                    ValuesToUpdate.Add("updated_by", ProductToUpdate.updated_by.ToString());
                }
                if (ProductToUpdate.updated_at != 0)
                {
                    ValuesToUpdate.Add("updated_at", ProductToUpdate.updated_at.ToString());
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
        Developer: Hamza Haq
        Date:5-7-19
        Action:get HTML Form (update Styles LINE ITEMS Data) from VIEW and pass the data to API product Repo 
        URL: /product/updateLineitems
        Input: HTML form Body Json with the data of new LINE ITEMS values and product_id
        output:current LINE ITEM id
        */
        // POST: api/product/create   // purchase order line items
        [HttpPost("updateLineitems")]
        public async Task<string> UpdateLineitemsAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrderLineItems newPOlineitems = JsonConvert.DeserializeObject<PurchaseOrderLineItems>(value);
                NewInsertionID = await _ProductRepo.UpdatePoLineItems(newPOlineitems);
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
          Developer: Mohi
          Date: 9-19-19 
          Action: Create product notes 
          URL: api/products/notecreate
          Request: Post
          Input: FromBody, string 
          output: string of Last insert product note id
          */
        [HttpPost("notecreate")]
        public async Task<string> NoteCreate([FromBody]   string value)
        {
            string newNoteID = "0";
            try
            {
                dynamic productNote = JsonConvert.DeserializeObject<Object>(value);
                int ref_id = productNote.ref_id;
                string note = productNote.note;
                int created_by = productNote.created_by;
                double created_at = _common.GetTimeStemp();
                newNoteID = await _NotesAndDoc.GenericPostNote<string>(ref_id, note_type, note, created_by, created_at);

                int userId = productNote.created_by;

                event_product_note_create = event_product_note_create.Replace("%%userid%%", userId.ToString());
                event_product_note_create = event_product_note_create.Replace("%%pid%%", ref_id.ToString());
                await _eventRepo.AddPhotoshootAsync(ref_id, event_product_note_create_id, ref_id, event_product_note_create, userId, _common.GetTimeStemp(), table_name);
                await _eventRepo.AddEventAsync(event_product_note_create_id, userId, ref_id, event_product_note_create, _common.GetTimeStemp(), user_event_table_name);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Photoshoot Note with message" + ex.Message);
            }
            return newNoteID;
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
        [HttpGet("delete/{product_id}/{po_id}")]
        public async Task<bool> DelAsync(string product_id,string po_id)
        {
            bool isDeleted = false;
            try
            {
                bool isItemDeleted = await _ItemsHelper.DeleteItemByProduct(product_id.ToString(), po_id);
                if (isItemDeleted)
                    isDeleted = await _ProductRepo.DeleteProduct(product_id, po_id);
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

        /*
       Developer: Hamza Haq
       Date: 9-03-19
       Request: GET
       Action:Get vendor Products for autocomplete
       URL: /product/getProductsbyVendor/{vendor_id}/{productname}
       Input: Vendor ID and product name
       output: productList
       */

        [HttpGet("getProductsbyVendor/{vendor_id}/{productname}")]
        public async Task<string> getProductsbyVendor(int vendor_id, string productname)
        {
            var ProductList = await _ProductRepo.GetProductsbyVendorAutoSuggest(vendor_id, productname);
            return JsonConvert.SerializeObject(ProductList);
        }

    }
}
