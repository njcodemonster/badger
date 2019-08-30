using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using itemService.Interfaces;
using GenericModals.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace itemService.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class ItemController : ControllerBase
    {

        private readonly ItemRepository _ItemRepository;
        ILoggerFactory _loggerFactory;

        public ItemController(ItemRepository _ItemReposit, ILoggerFactory loggerFactory)
        {
            _ItemRepository = _ItemReposit;
            _loggerFactory  = loggerFactory;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all list of items  "api/Item/list"
        URL: api/Item/list
        Request: Get
        Input: int Limit
        output: List of Items
        */
        [HttpGet("list/{Limit}")]
        public async Task<ActionResult<List<Items>>> GetAsync(int Limit)
        {
            List<Items> ToReturn = new List<Items>();
            try
            {
                return await _ItemRepository.GetAll(Limit);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all list of items by id "api/Item/list/id/1"
        URL: api/Item/list/id/1
        Request: Get
        Input: string ItemId
        output: List of Items
        */
        [HttpGet("list/id/{ItemId}")]
        public async Task<List<Items>> id(string ItemId)
        {   
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetItemById(ItemId);
                return ToRetrunItems;
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get Item with message" + ex.Message); 
                return ToRetrunItems;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all list of items by barcode "api/Item/list/barcode/1"
        URL: api/Item/list/barcode/1
        Request: Get
        Input: string barcode
        output: List of Items
        */
        [HttpGet("list/barcode/{Barcode}")]
        public async Task<List<Items>> barcode(string Barcode)
        {  
            List<Items> ToRetrun = new List<Items>();
            try
            {
                ToRetrun = await _ItemRepository.GetItemByBarcode(Barcode);
                return ToRetrun;
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get Item by barcode with message" + ex.Message);
                return ToRetrun;
            }

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all list of items by bagNumber "api/Item/list/bagNumber/1"
        URL: api/Item/list/bagNumber/1
        Request: Get
        Input: string bagNumber
        output: List of Items
        */
        [HttpGet("list/bagNumber/{BagNumber}")]
        public async Task<List<Items>> BagNumber(string BagNumber)
        {
            List<Items> ToRetrun = new List<Items>();
            try
            {
                 ToRetrun = await _ItemRepository.GetItemByBagNumber(BagNumber);
                return ToRetrun;
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get Item by bagNumber with message" + ex.Message);
                return ToRetrun;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all list of items by skuFamily and limit "api/Item/list/bagNumber/1/limit"
        URL: api/Item/list/skuFamily/1/limit
        Request: Get
        Input: string skuFamily, int limit
        output: List of Items
        */
        [HttpGet("list/skuFamily/{skuFamily}/{Limit}")]
        public async Task<List<Items>> skuFamily(string skuFamily, int Limit )
        {
            List<Items> ToRetrun = new List<Items>();
            try
            {
                ToRetrun = await _ItemRepository.GetBySkuFamily(skuFamily, Limit);
                return ToRetrun;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrun;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all list of items by productId and limit "api/Item/list/productId/1/limit"
        URL: api/Item/list/productId/1/limit
        Request: Get
        Input: string productId, int limit
        output: List of Items
        */
        [HttpGet("list/productId/{ProductId}/{Limit}")]
        public async Task<List<Items>> productId(string ProductId, int Limit)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetByProductId(ProductId, Limit);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }

        /*
       Developer: Sajid Khan
       Date: 7-5-19 
       Action: Get all list of items by statusId and limit "api/Item/list/statusId/1/limit"
       URL: api/Item/list/statusId/1/limit
       Request: Get
       Input: string statusId, int limit
       output: List of Items
       */
        [HttpGet("list/statusId/{StatusId}/{Limit}")]
        public async Task<List<Items>> statusId(string StatusId, int Limit)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetByStatusId(StatusId, Limit);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }

        /*
       Developer: Sajid Khan
       Date: 7-5-19 
       Action: Get all list of items by vendorId and limit "api/Item/list/vendorId/1/limit"
       URL: api/Item/list/vendorId/1/limit
       Request: Get
       Input: string vendorId, int limit
       output: List of Items
       */
        [HttpGet("list/vendorId/{VendorId}/{Limit}")]
        public async Task<List<Items>> vendorId(string VendorId, int Limit)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetByVendorId(VendorId, Limit);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }

        /*
      Developer: Sajid Khan
      Date: 7-5-19 
      Action: Get all list of items by skuId and limit "api/Item/list/skuId/1/limit"
      URL: api/Item/list/skuId/1/limit
      Request: Get
      Input: string skuId, int limit
      output: List of Items
      */
        [HttpGet("list/skuId/{SkuId}/{Limit}")]
        public async Task<List<Items>> skuId(string SkuId, int Limit)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetBySkuId(SkuId, Limit);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }

        /*
         Developer: Sajid Khan
         Date: 7-5-19 
         Action: Get all list of items by publishDate and limit "api/Item/list/publishDate/1/limit"
         URL: api/Item/list/publishDate/1/limit
         Request: Get
         Input: string publishDate, int limit
         output: List of Items
         */
        [HttpGet("list/publishDate/{PublishDate}/{Limit}")]
        public async Task<List<Items>> publishDate(string PublishDate, int Limit)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetByPublishDate(PublishDate, Limit);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all list of items by poid and limit "api/Item/list/listforPO/1/limit"
        URL: api/Item/list/listforPO/1/limit
        Request: Get
        Input: int poid, int limit
        output: List of Items
        */
        [HttpGet("list/listforPO/{PO_id}")]
        public async Task<List<Items>> GetItemsByPo(int PO_id)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetByPOid(PO_id);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }

        
        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all list of items by poid and limit "api/Item/list/listforPO/1/limit"
        URL: api/Item/list/listforPO/1/limit
        Request: Get
        Input: int poid, int limit
        output: List of Items
        */
        [HttpGet("list/getitemsgroupbyproductid/{PO_id}")]
        public async Task<List<Items>> GetItemsGroupByProductId(int PO_id)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetItemGroupByProductId(PO_id);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }



        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all list of items by publishDateRange and limit "api/Item/list/publishDateRange/start/end/limit"
        URL: api/Item/list/publishDateRange/start/end/limit
        Request: Get
        Input: string StartDate, string EndDate, int Limit
        output: List of Items
        */
        [HttpGet("list/publishDateRange/{StartDate}/{EndDate}/{Limit}")]
        public async Task<List<Items>> publishDateRange(string StartDate, string EndDate, int Limit)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetByPublishDateRange(StartDate, EndDate, Limit);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all list of items by raStatus and limit "api/Item/list/raStatus/1/limit"
        URL: api/Item/list/raStatus/1/limit
        Request: Get
        Input: string RaStatus, int Limit
        output: List of Items
        */
        [HttpGet("list/raStatus/{RaStatus}/{Limit}")]
        public async Task<List<Items>> raStatus(string RaStatus, int Limit)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetByRaStatus(RaStatus, Limit);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }

        /*
       Developer: Sajid Khan
       Date: 7-5-19 
       Action: Get all list of items by publishedBy and limit "api/Item/list/publishedBy/1/limit"
       URL: api/Item/list/publishedBy/1/limit
       Request: Get
       Input: string publishedBy, int Limit
       output: List of Items
       */
        [HttpGet("list/publishedBy/{PublishedBy}/{Limit}")]
        public async Task<List<Items>> publishedBy(string PublishedBy, int Limit)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetByRaStatus(PublishedBy, Limit);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }

        /*
      Developer: Sajid Khan
      Date: 7-5-19 
      Action: Get all list of items by updateDateRange and limit "api/Item/list/updateDateRange/start/end/limit"
      URL: api/Item/list/updateDateRange/start/end/limit
      Request: Get
      Input: string StartDate, string EndDate, int Limit
      output: List of Items
      */
        [HttpGet("list/updateDateRange/{StartDate}/{EndDate}/{Limit}")]
        public async Task<List<Items>> updateDateRange(string StartDate, string EndDate, int Limit)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetByUpdateDateRange(StartDate, EndDate, Limit);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }

        /*
      Developer: Sajid Khan
      Date: 7-5-19 
      Action: Get all list of items by createDateRange and limit "api/Item/list/createDateRange/start/end/limit"
      URL: api/Item/list/createDateRange/start/end/limit
      Request: Get
      Input: string StartDate, string EndDate, int Limit
      output: List of Items
      */
        [HttpGet("list/createDateRange/{StartDate}/{EndDate}/{Limit}")]
        public async Task<List<Items>> createDateRange(string StartDate, string EndDate, int Limit)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetByCreateDateRange(StartDate, EndDate, Limit);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all list of items by afterDate and limit "api/Item/list/afterDate/afterDate/limit"
        URL: api/Item/list/afterDate/afterDate/limit
        Request: Get
        Input: string AfterDate, int Limit
        output: List of Items
        */
        [HttpGet("list/afterDate/{AfterDate}/{Limit}")]
        public async Task<List<Items>> afterDate(string AfterDate, int Limit)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetAfterDate(AfterDate, Limit);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all list of items by product_id and limit "api/Item/list/ProductItemSentToPhotoshoot/product_id"
        URL: api/Item/list/ProductItemSentToPhotoshoot/product_id
        Request: Get
        Input: string product_id
        output: List of Items
        */
        [HttpGet("list/ProductItemSentToPhotoshoot/{product_id}")]
        public async Task<string> ProductItemSentToPhotoshoot(string product_id)
        {
            string UpdateResult = "Success";
            try
            {
                await _ItemRepository.SetProductItemSentToPhotoshoot(product_id);
                return UpdateResult;
            }
            catch (Exception skuFamilyException)
            {
                return "Failed";
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all list of items by beforeDate and limit "api/Item/list/beforeDate/1/limit"
        URL: api/Item/list/beforeDate/1/limit
        Request: Get
        Input: string BeforeDate, int Limit
        output: List of Items
        */
        [HttpGet("list/beforeDate/{BeforeDate}/{Limit}")]
        public async Task<List<Items>> beforeDate(string BeforeDate, int Limit)
        {
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.GetBeforeDate(BeforeDate, Limit);
                return ToRetrunItems;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrunItems;
            }
        }

        /*Developer: ubaid
        Date:5-7-19
        Action:get HTML Form(New items Data) from badger api and pass the data to items Repo
        URL: /item/create/{quantity}
        Input: HTML form Body Json with the data of new item(s)
        output: New item id
        */
        [HttpPost("create/{qty}")]
        public async Task<string> PostAsync([FromBody]   string value,int qty = 0)
        {
            string NewInsertionID = "0";
            try
            {
                Items newItems = JsonConvert.DeserializeObject<Items>(value);
                for (int loop = 0; loop < qty; loop++)
                {
                    NewInsertionID = await _ItemRepository.Create(newItems);
                }
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Item with message" + ex.Message);
            }
            return NewInsertionID;
        }
        /*Developer: Hamza Haq
        Date:30-8-19
        Action:get HTML Form(uodate items Data) from badger api and pass the data to items Repo
        URL: /item/update/{quantity}
        Input: HTML form Body Json with the data of new item(s)
        output: New item id
        */
        [HttpPost("update/{qty}")]
        public async Task<string> updatePostAsync([FromBody]   string value, int qty = 0)
        {
            string NewInsertionID = "0";
            try
            {
                Items newItems = JsonConvert.DeserializeObject<Items>(value);
                if (qty > newItems.original_qty)
                {
                   qty -= newItems.original_qty;
                }
                else
                {
                    //Delete items logic
                }

                for (int loop = 0; loop < qty; loop++)
                {
                    NewInsertionID = await _ItemRepository.Create(newItems);
                }
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Item with message" + ex.Message);
            }
            return NewInsertionID;
        }


        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update product items by status and limit "api/Item/UpdateProductItemForPhotoshoot/status"
        URL: api/Item/UpdateProductItemForPhotoshoot/status
        Request: Post
        Input: [FromBody] string value, int status
        output: string
        */
        [HttpPost("UpdateProductItemForPhotoshoot/{status}")]
        public async Task<string> UpdateProductItemForPhotoshoot([FromBody]   string value, int status)
        {
            string toReturn= "success";
            string response = "";
            try
            {
                dynamic ProductSkuList  = JsonConvert.DeserializeObject(value);
                foreach (var ExpendJson in ProductSkuList)
                {
                    string SkuListString = ExpendJson.ToString();
                    int splidIds = SkuListString.Count(c => c == ':');
                    if (splidIds > 0) {
                        string ids = SkuListString.Split(":").Last();
                        ids = ids.Replace("\"", "");
                        var idsList = ids.Split(",");
                        foreach (var skuid in idsList)
                        {
                            int skuIdValue = Int32.Parse(skuid.ToString().Trim());
                            response = await _ItemRepository.SetProductItemForPhotoshoot(skuIdValue, status);
                            if (response == "success") {
                                break;
                            }
                        }
                    }
                }

                toReturn = response;
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Item with message" + ex.Message);
                toReturn = "failed";
            }
            return toReturn;
        }


        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update product items by id and limit "api/Item/update/1"
        URL: api/Item/update/1
        Request: Put
        Input: int id, [FromBody] string value
        output: string
        */
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        { 
            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                Items ItemToUpdate = JsonConvert.DeserializeObject<Items>(value);
                ItemToUpdate.item_id = id;
                UpdateProcessOutput = await _ItemRepository.Update(ItemToUpdate);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  item with message" + ex.Message);
                UpdateResult = "Failed";
            }
            if (!UpdateProcessOutput)
            {
                UpdateResult = "Creation failed due to reason: No specific reson";
            }
            return UpdateResult;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update product items barcode by id and limit "api/Item/updateBarcode/id/barcode"
        URL: api/Item/updateBarcode/id/barcode
        Request: Get
        Input: string Barcode, int id
        output: string
        */
        [HttpGet("updateBarcode/{id}/{Barcode}")]
        public async Task<string> updateBarcode(string Barcode, int id)
        {
            string UpdateResult = "Success";
            try
            {
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
                ValuesToUpdate.Add("barcode", Barcode);

                await _ItemRepository.UpdateSpeific(ValuesToUpdate, "item_id=" + id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  item barcode with message" + ex.Message);
                UpdateResult = "Failed";
            }

            return UpdateResult;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update product items BagNumber by id and limit "api/Item/updateBagNumber/id/BagNumber"
        URL: api/Item/updateBagNumber/id/BagNumber
        Request: Get
        Input: string BagNumber, int id
        output: string
        */
        [HttpGet("updateBagNumber/{id}/{BagNumber}")]
        public async Task<string> updateBagNumber(string BagNumber, int id)
        {
            string UpdateResult = "Success";
            try
            {
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
                ValuesToUpdate.Add("bag_code", BagNumber);
                await _ItemRepository.UpdateSpeific(ValuesToUpdate, "item_id=" + id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  item bagNumber with message" + ex.Message);
                UpdateResult = "Failed";
            }

            return UpdateResult;
        }

        /*
       Developer: Sajid Khan
       Date: 7-5-19 
       Action: update product items slot by id and limit "api/Item/updateSlot/id/slot"
       URL: api/Item/updateSlot/id/slot
       Request: Get
       Input: string Slot, int id
       output: string
       */
        [HttpGet("updateSlot/{id}/{Slot}")]
        public async Task<string> updateSlot(string Slot, int id)
        {
            string UpdateResult = "Success";
            try
            {
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
                ValuesToUpdate.Add("slot_number", Slot);
                await _ItemRepository.UpdateSpeific(ValuesToUpdate, "item_id=" + id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  item slot number with message" + ex.Message);
                UpdateResult = "Failed";
            }

            return UpdateResult;
        }

        /*
          Developer: Sajid Khan
          Date: 7-5-19 
          Action: update specific product items by id "api/Item/specificUpdate/id"
          URL: api/Item/specificUpdate/id
          Request: Put
          Input: int id, [FromBody] string value
          output: string
          */
        [HttpPut("specificUpdate/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {
            string UpdateResult = "Success";

            try
            {
                Items ItemToUpdate = JsonConvert.DeserializeObject<Items>(value);
                ItemToUpdate.item_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
                if (ItemToUpdate.barcode != 0)
                {
                    ValuesToUpdate.Add("barcode", ItemToUpdate.barcode.ToString());
                }
                if (ItemToUpdate.slot_number != null)
                {
                    ValuesToUpdate.Add("slot_number", ItemToUpdate.slot_number.ToString());
                }
                if (ItemToUpdate.bag_code != null)
                {
                    ValuesToUpdate.Add("vendor_description", ItemToUpdate.bag_code.ToString());
                }
                if (ItemToUpdate.item_status_id != 0)
                {
                    ValuesToUpdate.Add("item_status_id", ItemToUpdate.item_status_id.ToString());
                }
                if (ItemToUpdate.ra_status != 0)
                {
                    ValuesToUpdate.Add("ra_status", ItemToUpdate.ra_status.ToString());
                }
                if (ItemToUpdate.sku != null)
                {
                    ValuesToUpdate.Add("sku", ItemToUpdate.sku);
                }
                if (ItemToUpdate.sku_id != 0)
                {
                    ValuesToUpdate.Add("sku_id", ItemToUpdate.sku_id.ToString());
                } 
                if (ItemToUpdate.product_id != 0)
                {
                    ValuesToUpdate.Add("product_id", ItemToUpdate.product_id.ToString());
                }
                if (ItemToUpdate.vendor_id != 0)
                {
                    ValuesToUpdate.Add("vendor_id", ItemToUpdate.vendor_id.ToString());
                }
                if (ItemToUpdate.sku_family != null)
                {
                    ValuesToUpdate.Add("sku_family", ItemToUpdate.sku_family);
                }
                if (ItemToUpdate.published != 0)
                {
                    ValuesToUpdate.Add("published", ItemToUpdate.published.ToString());
                }
                if (ItemToUpdate.published_by != 0)
                {
                    ValuesToUpdate.Add("published_by", ItemToUpdate.published_by.ToString());
                }
                if (ItemToUpdate.created_by != 0)
                {
                    ValuesToUpdate.Add("created_by", ItemToUpdate.created_by.ToString());
                }
                if (ItemToUpdate.updated_by != 0)
                {
                    ValuesToUpdate.Add("updated_by", ItemToUpdate.updated_by.ToString());
                }
                if (ItemToUpdate.created_at != 0)
                {
                    ValuesToUpdate.Add("created_at", ItemToUpdate.created_at.ToString());
                }
                if (ItemToUpdate.updated_at != 0)
                {
                    ValuesToUpdate.Add("updated_at", ItemToUpdate.updated_at.ToString());
                }

                 await _ItemRepository.UpdateSpeific(ValuesToUpdate, "item_id=" + id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new item with message" + ex.Message);
                UpdateResult = "Failed";
            }

            return UpdateResult;
        }

        /*
        Developer: Sajid Khan
        Date: 7-20-19 
        Action: Check Barcode already exist by barcode 
        URL: api/item/checkbarcodeexist/12345678
        Request: Get
        Input: int barcode
        output: boolean
        */
        [HttpGet("checkbarcodeexist/{barcode}")]
        public async Task<Boolean> CheckBarcodeExist(int barcode)
        {
            Boolean result = false;
            List<Items> ToReturn = new List<Items>();
            try
            {
                ToReturn = await _ItemRepository.CheckBarcodeExist(barcode);

                if (ToReturn.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return result;
        }

        /*
        Developer: Sajid Khan
        Date: 08-09-19 
        Action: get item data with Barcode by barcode 
        URL: api/item/getbarcode/12345678
        Request: Get
        Input: int barcode
        output: boolean
        */
        [HttpGet("getbarcode/{barcode}")]
        public async Task<object> GetBarcode(int barcode)
        {
            dynamic barcodeDetails = new object();
            try
            {
                barcodeDetails = await _ItemRepository.GetBarcode(barcode);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get barcode with message" + ex.Message);

            }

            return barcodeDetails;
        }

        /*
     Developer: Rizwan ali
     Date: 08-09-19 
     Action: delete item by product
     URL: api/item/deleteItemByProduct/?
     Request: Get
     Input: string product id
     output: bool status
     */
        [HttpDelete("deleteItemByProduct/{id}")]
        public async Task<bool> DeleteItemByProduct(string id)
        {
            bool status = false;
            try
            {
                status = await _ItemRepository.DeleteItemByProduct(id);

            }
            catch (Exception ex)
            {
                status = false;
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get barcode with message" + ex.Message);

            }

            return status;
        }
    }
}
