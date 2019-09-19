using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using badgerApi.Interfaces;
using GenericModals;
using GenericModals.Extentions;
using GenericModals.Reports;
using Microsoft.AspNetCore.Mvc;

namespace badgerApi.Controllers
{
    [Route("api/report")]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _reportRepo;
        public ReportController(IReportRepository reportRepo)
        {
            _reportRepo = reportRepo;
        }

        [HttpPost("GetPOCountByUserReport")]
        public async Task<ResponseModel> GetPOCountByUserReport([FromBody] DataTableAjaxModel dataTable)
        {
            var report = await _reportRepo.GetPoCountByUsersReportAsync(dataTable);
            return ResponseHelper.GetResponse(report);
        }
    }
}