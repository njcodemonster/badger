using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using badger_view.Helpers;
using GenericModals.Models;
using System.Dynamic;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace badger_view.Controllers
{
    public class AttributesController : Controller
    {
        private BadgerApiHelper _BadgerApiHelper;
        public AttributesController(ILoginHelper LoginHelper,BadgerApiHelper badgerApiHelper)
        {
            _BadgerApiHelper = badgerApiHelper;
        }

        /*
        Developer: Hamza Haq
        Date:9-04-19 
        Action: Get All sku sizes from database
        URL: attributes/getskusizes
        Input: None
        output: Categories list
        */
        [Authorize]
        [HttpGet("attributes/getskusizes")]
        public async Task<object> GetSkuSizes()
        {
            var skuSizesList = await _BadgerApiHelper.GenericGetAsync<object>("/attributes/list/type/1");
            return skuSizesList;
        }

        /*
        Developer: Hamza Haq
        Date:9-21-19 
        Action: Get All fabrics from database
        URL: attributes/getfabrics/{name}
        Input: None
        output: get fabric list
        */
        [Authorize]
        [HttpGet("attributes/getfabrics/{name}")]
        public async Task<object> GetFabrics(string name)
        {
            SetBadgerHelper();
            var fabricLists = await _BadgerApiHelper.GenericGetAsync<object>("/attributes/list/type/5/" + name + "");
            return fabricLists;
        }

    }
}
