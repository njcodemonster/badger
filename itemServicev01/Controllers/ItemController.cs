using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using itemService.Interfaces;
using itemService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace itemService.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class ItemController : ControllerBase
    {

        private readonly ItemRepository _ItemRepository;

        public ItemController(ItemRepository _ItemReposit)
        {
            _ItemRepository = _ItemReposit;
        }
        
        // GET: /id/5
        [HttpGet("list/id/{ItemId}")]
        public async Task<List<Items>> id(string ItemId)
        {   
            List<Items> ToRetrunItems = new List<Items>();
            try
            {
                ToRetrunItems = await _ItemRepository.getItemById(ItemId);
                return ToRetrunItems;
            }
            catch (Exception IdException)
            {
                return ToRetrunItems;
            }
        }

        [HttpGet("list/barcode/{Barcode}")]
        public async Task<List<Items>> barcode(string Barcode)
        {  
            List<Items> ToRetrun = new List<Items>();
            try
            {
                ToRetrun = await _ItemRepository.getItemByBarcode(Barcode);
                return ToRetrun;
            }
            catch (Exception BarcodeException)
            {
                return ToRetrun;
            }

        }

        [HttpGet("list/bagNumber/{BagNumber}")]
        public async Task<List<Items>> BagNumber(string BagNumber)
        {
            List<Items> ToRetrun = new List<Items>();
            try
            {
                 ToRetrun = await _ItemRepository.getItemByBagNumber(BagNumber);
                return ToRetrun;
            }
            catch(Exception BagNumberException)
            {
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
