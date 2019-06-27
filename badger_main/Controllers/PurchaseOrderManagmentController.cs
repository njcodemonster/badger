using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using badgerApi.Helper;
using badgerApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderManagementController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IPurchaseOrdersRepository _PurchaseOrdersRepo;
        ILoggerFactory _loggerFactory;
        private INotesAndDocHelper _NotesAndDoc;
        private int note_type = 4;
        private IItemServiceHelper _ItemsHelper;
        public PurchaseOrderManagementController(IPurchaseOrdersRepository PurchaseOrdersRepo, ILoggerFactory loggerFactory, INotesAndDocHelper NotesAndDoc, IConfiguration config, IItemServiceHelper ItemsHelper)
        {
            _config = config;
            _PurchaseOrdersRepo = PurchaseOrdersRepo;
            _loggerFactory = loggerFactory;
            _NotesAndDoc = NotesAndDoc;
            _ItemsHelper = ItemsHelper;
        }

        private int FindSku(dynamic LineItemArray,String sku)
        {
            int index = -1;
            foreach(dynamic LineItem in LineItemArray)
            {
                if(sku == (String)LineItem.sku)
                {
                    index++;
                    break;
                }
                index++;
            }
            return index;
        }
        [HttpGet("ListAllItemStatus")]
        public async Task<object> ListAllItemStatus()
        {
            dynamic AllItemStatus = new object();
            try
            {
                AllItemStatus = await _ItemsHelper.GetAllStatus();
            }
            catch (Exception ex)
            {   
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for all Item status from Item service with message" + ex.Message);
            }
            return AllItemStatus;

        }
        [HttpGet("GetLineItemDetails/{PO_id}/{limit}")]
        public async Task<object> GetLineItemsDetails(int PO_id, int limit)
        {
            dynamic LineITemsDetails = new object();
            try
            {
                LineITemsDetails = await _PurchaseOrdersRepo.GetOpenPOLineItemDetails(PO_id,limit);
                List<Items> items = await _ItemsHelper.GetItemsByOrder(PO_id);
                List<Items> skuItems = new List<Items>();
                string currentSKU = "";
                Boolean First = true;
                int Index = -1;
                foreach (Items item in items)
                {
                    if(item.sku != currentSKU)
                    {
                        
                        if (!First)
                        {
                            Index = FindSku(LineITemsDetails.LineItemDetails, currentSKU);
                            if(Index != -1)
                            {
                                LineITemsDetails.LineItemDetails[Index].EndItems = skuItems;
                            }
                        }
                        currentSKU = item.sku;
                        First = false;
                        skuItems = new List<Items>();
                    }
                    skuItems.Add(item);
                   

                }
                Index = FindSku(LineITemsDetails.LineItemDetails, currentSKU);
                if (Index != -1)
                {
                    LineITemsDetails.LineItemDetails[Index].EndItems = skuItems;
                }
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetLineItemsDetails for PO : "+PO_id.ToString()+" with message" + ex.Message);
            }

            return LineITemsDetails;
        }


    }
}