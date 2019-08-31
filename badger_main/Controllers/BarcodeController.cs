using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using badgerApi.Helper;
using badgerApi.Interfaces;
using GenericModals.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarcodeController : Controller
    {
        private readonly iBarcodeRangeRepo _BarcodeRangeRepo;
        private IItemServiceHelper _ItemsHelper;
        ILoggerFactory _loggerFactory;
        INotesAndDocHelper _notesAndDocHelper;
        public BarcodeController(iBarcodeRangeRepo BarcodeRangeRepo, ILoggerFactory loggerFactory, INotesAndDocHelper notesAndDocHelper, IItemServiceHelper ItemsHelper)
        {
            _ItemsHelper = ItemsHelper;
            _BarcodeRangeRepo = BarcodeRangeRepo;
            _loggerFactory = loggerFactory;
            _notesAndDocHelper = notesAndDocHelper;
        }

        /*
       Developer: Rizwan Ali
       Date: 9-8-19 
       Action: Get barcode ranges all that are in the system  "api/purchaseorders/getBarcodeRange"
       URL: api/purchaseorders/listpageview/10/boolean
       Request: Get
       Input: none
       output: list of barcode ranges all that are in the system
       */
        [HttpGet("getBarcodeRange/{start}/{limit}")]
        public async Task<object> GetBarcodeRange(int start, int limit)
        {
            dynamic barcodes = new object();
            try
            {
                barcodes = await _BarcodeRangeRepo.GetBarcodeRangeList(start, limit);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in fetching the data for barcode range with message" + ex.Message);

            }

            return barcodes;

        }        /*
       Developer: Rizwan Ali
       Date: 9-8-19 
       Action: Get barcode ranges all that are in the system  "api/purchaseorders/getBarcodeRange"
       URL: api/purchaseorders/listpageview/10/boolean
       Request: Get
       Input: none
       output: list of barcode ranges all that are in the system
       */
        [HttpGet("deletebarcode/{id}")]
        public async Task<bool> DeleteBarcode(int id)
        {
           
            try
            {
                return await _BarcodeRangeRepo.DeleteBarcode(id);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in fetching the data for barcode range with message" + ex.Message);
                return false;
            }

            return false; ;

        }
        /*
        Developer: Rizwan Ali
        Date: 9-8-19 
        Action: Get barcode ranges all that are in the system  "api/purchaseorders/getBarcodeRange"
        URL: api/purchaseorders/listpageview/10/boolean
        Request: Get
        Input: none
        output: list of barcode ranges all that are in the system
        */
        [HttpPost("validate")]
        public async Task<string> Validate([FromBody]   string value)
        {
            bool status = false;
            try
            {
                Barcode barcode = JsonConvert.DeserializeObject<Barcode>(value);
                status = await _BarcodeRangeRepo.Validate(barcode);
            }
            catch (Exception ex)
            {
                status = false;
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Parent Category with message" + ex.Message);
            }
            return status.ToString();
        }
        
        /*
        Developer: Rizwan Ali
        Date: 9-8-19 
        Action: Get barcode ranges all that are in the system  "api/purchaseorders/getBarcodeRange"
        URL: api/purchaseorders/listpageview/10/boolean
        Request: Get
        Input: none
        output: list of barcode ranges all that are in the system
        */
        [HttpPost("createorupdate")]
        public async Task<string> Create([FromBody]   string value)
        {
            string status = "false";
            try
            {
                Barcode barcode = JsonConvert.DeserializeObject<Barcode>(value);
                status = await _BarcodeRangeRepo.CreateOrUpdate(barcode);
            }
            catch (Exception ex)
            {
                status = "false";
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Parent Category with message" + ex.Message);
            }
            return status.ToString();
        }


    }
}