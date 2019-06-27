using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using itemService.Interfaces;
using itemService.Models;
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

        // GET: /id/5
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
        // GET: api/Items
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        
        // GET: api/Items/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Items
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Items/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }
}
