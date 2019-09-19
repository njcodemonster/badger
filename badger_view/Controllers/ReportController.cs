using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using badger_view.Helpers;
using GenericModals.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace badger_view.Controllers
{
    [Route("report")]
    public class ReportController : Controller
    {
        private BadgerApiHelper _badgerApiHelper;
        public ReportController(IConfiguration config)
        {
            _badgerApiHelper = new BadgerApiHelper(config);
        }
        public IActionResult POCountByUser()
        {
            return View();
        }

        [HttpPost("GetPOCountByUser")]
        public async Task<JsonResult> GetPOCountByUser([FromForm] DataTableAjaxModel dataTable)
        {
            var report = await _badgerApiHelper.PostAsync<JQDataTableResponse>(dataTable, "/report/GetPOCountByUserReport");
            return Json(report);
        }
    }
}