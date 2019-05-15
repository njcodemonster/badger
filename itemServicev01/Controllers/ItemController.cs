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
        [HttpGet("id/{ItemId}")]
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


        [HttpGet("barcode/{Barcode}")]
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

        [HttpGet("bagNumber/{BagNumber}")]
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

        [HttpGet("skuFamily/{skuFamily}")]
        public async Task<List<Items>> skuFamily(string skuFamily)
        {
            List<Items> ToRetrun = new List<Items>();
            try
            {
                ToRetrun = await _ItemRepository.GetBySkuFamily(skuFamily);
                return ToRetrun;
            }
            catch (Exception skuFamilyException)
            {
                return ToRetrun;
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
