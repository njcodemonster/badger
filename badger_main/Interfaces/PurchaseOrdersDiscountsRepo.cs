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
    public interface IPurchaseOrdersDiscountsRepository
    {
        Task<PurchaseOrderDiscounts> GetById(int id);
        Task<List<PurchaseOrderDiscounts>> GetAll(Int32 Limit);
        Task<String> Create(PurchaseOrderDiscounts NewPurchaseOrder);
        Task<Boolean> Update(PurchaseOrderDiscounts PurchaseOrdersToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task<string> Count();
        Task<object> GetPurchaseOrdersDiscount(int id);

    }

    public class PurchaseOrdersDiscountsRepo : IPurchaseOrdersDiscountsRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "purchase_order_discounts";
        private string selectlimit = "30";
        public PurchaseOrdersDiscountsRepo(IConfiguration config)
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
        Action: Insert purchase order discount data to database
        Input: new Purchase Orders discount data
        output: string of PurchaseOrders discount id
        */
        public async Task<string> Create(PurchaseOrderDiscounts NewPurchaseOrder)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<PurchaseOrderDiscounts>(NewPurchaseOrder);
                return result.ToString();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Total count of purchase order discount data from database
        Input: null
        output: string of PurchaseOrders discount count
        */
        public async Task<string> Count()
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync<String>("select count(po_discount_id) from " + TableName + ";");
                return result.FirstOrDefault();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order discount data by poid from database
        Input: int poid
        output: dynamic object of Purchase Orders discount
        */
        public async Task<object> GetPurchaseOrdersDiscount(int id)
        {
            dynamic poLedger = new ExpandoObject();
            string sQuery = "";

            sQuery = "SELECT * from " + TableName + " WHERE po_id= " + id.ToString() + " LIMIT 1;";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> purchaseOrdersLedger = await conn.QueryAsync<object>(sQuery);
                poLedger = purchaseOrdersLedger;
            }
            return poLedger;

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get All purchase order discount data from database
        Input: int limit
        output: list of Purchase Orders discount
        */
        public async Task<List<PurchaseOrderDiscounts>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<PurchaseOrderDiscounts> result = new List<PurchaseOrderDiscounts>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<PurchaseOrderDiscounts>("Select * from " + TableName + " Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<PurchaseOrderDiscounts>();
                }
                return result.ToList();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order discount data by id from database
        Input: int id
        output: list of PurchaseOrders discount
        */
        public async Task<PurchaseOrderDiscounts> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<PurchaseOrderDiscounts>(id);
                return result;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update purchase order discount data by id from database
        Input: purchase order discount data
        output: Boolean
        */
        public async Task<Boolean> Update(PurchaseOrderDiscounts PurchaseOrdersToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<PurchaseOrderDiscounts>(PurchaseOrdersToUpdate);
                return result;
            }

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Update Specific purchase order discount data by id from database
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
