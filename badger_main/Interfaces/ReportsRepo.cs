using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using GenericModals.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using CommonHelper;
using System.Dynamic;
using GenericModals.Reports;
using badgerApi.Helpers;
using GenericModals.Extentions;

namespace badgerApi.Interfaces
{ 
    public interface IReportRepository
    {
        Task<List<PoCountByUser>> GetPOCountByUserReportAsync(DataTableAjaxModel dataTable);
        Task<JQDataTableResponse> GetPoCountByUsersReportAsync(DataTableAjaxModel dataTable);
    }
    public class ReportsRepo : IReportRepository
    {
        private readonly DataAccessLayer _db;
        private string TableName = "vendor";
        private string selectlimit = "30";
        public ReportsRepo(IConfiguration config)
        {
            _db = new DataAccessLayer(config, "ProductsDatabase");
           // selectlimit = _config.GetValue<string>("configs:Default_select_Limit");
        }

        /*
        Developer: M.Usama
        Date: 05-09-2019
        Action: Get PO Report by user
        Input: null
        output: list of report data
         */
        public async Task<List<PoCountByUser>> GetPOCountByUserReportAsync(DataTableAjaxModel dataTable)
        {
            string procedureName = "sp_get_po_count_by_users";
            int total_count = 0;
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@page_size", dataTable.length, DbType.Int32);
            sqlParameters.Add("@offset_value", dataTable.start, DbType.Int32);
            sqlParameters.Add("@order_by", dataTable.columns[dataTable.order[0].column].name, DbType.String);
            sqlParameters.Add("@sort", dataTable.order[0].dir, DbType.String);
            sqlParameters.Add("@search", dataTable.search.value, DbType.String);
            sqlParameters.Add("@total_count", total_count, DbType.Int32, ParameterDirection.Output);
            var report = await _db.ExecuteProcedureAsync<PoCountByUser>(procedureName, sqlParameters);
            return report.ToList();
        }

        public async Task<JQDataTableResponse> GetPoCountByUsersReportAsync(DataTableAjaxModel dataTable)
        {
            string orderBy = dataTable.columns[dataTable.order[0].column].name;
            string sortBy = dataTable.order[0].dir;
            string whereClause = $"WHERE created_at BETWEEN '{dataTable.from_date}' AND '{dataTable.to_date}'";
            whereClause += dataTable.search.value == null ? string.Empty
                : " AND po_count LIKE '%" + dataTable.search.value + "%' OR username LIKE '%" + dataTable.search.value + "%'";

            string query = @"SELECT SQL_CALC_FOUND_ROWS * FROM po_count_by_users_report " +
                            whereClause +
                            " ORDER BY " + orderBy + " " + sortBy +
                            " LIMIT " + dataTable.length + " OFFSET " + dataTable.start + ";" +
                            " SELECT FOUND_ROWS();";

            (IEnumerable<PoCountByUser> list, long count) reportData = await _db.GetByMultiQuery<PoCountByUser, long>(query);

            var jqGridResponse = new JQDataTableResponse
            {
                data = reportData.list,
                draw = dataTable.draw,
                recordsFiltered = reportData.count,
                recordsTotal = reportData.Item2,
                status = "Success"
            };
            return jqGridResponse;
        }
        

    }
}

