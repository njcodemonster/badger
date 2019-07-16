using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using badgerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using CommonHelper;
using System.Dynamic;

namespace badgerApi.Interfaces
{
    public interface IPurchaseOrdersTrackingRepository
    {
        Task<object> GetTrackingById(int id);
        Task<String> Create(PurchaseOrdersTracking Newtracking);
        Task<Boolean> Update(PurchaseOrdersTracking TrackingToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
    }
    public class PurchaseOrdersTrackingRepo : IPurchaseOrdersTrackingRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "purchase_order_tracking";
        private string selectlimit = "30";
        public PurchaseOrdersTrackingRepo(IConfiguration config)
        {

            _config = config;
            selectlimit = _config.GetValue<string>("configs:Default_select_Limit");

        }
        public IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(_config.GetConnectionString("ProductsDatabase"));
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: get tracking by id from database
        Input: int id
        output: dynamic object of tracking
        */
        public async Task<object> GetTrackingById(int id)
        {
            dynamic poPageList = new ExpandoObject();
            string sQuery = "";

            sQuery = "SELECT * from purchase_order_tracking Where po_id = " + id;
           
            using (IDbConnection conn = Connection)
            {
               poPageList = await conn.QueryAsync<object>(sQuery);
            }
            return poPageList;

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Insert tracking to database
        Input: new tracking data
        output: string of tracking id
        */
        public async Task<string> Create(PurchaseOrdersTracking Newtracking)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<PurchaseOrdersTracking>(Newtracking);
                return result.ToString();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action:update tracking data by id from database
        Input: tracking data
        output:Boolean
        */
        public async Task<Boolean> Update(PurchaseOrdersTracking TrackingToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<PurchaseOrdersTracking>(TrackingToUpdate);
                return result;
            }

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action:update specific tracking data by id from database
        Input: fields value and where to update
        output: Boolean
        */
        public async Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where)
        {
            QueryHelper qHellper = new QueryHelper();
            string UpdateQuery = qHellper.MakeUpdateQuery(ValuePairs, TableName, where);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync(UpdateQuery);

            }

        }
        
    }
}
