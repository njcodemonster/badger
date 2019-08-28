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

namespace badgerApi.Interfaces
{    public interface IPurchaseOrdersLineItemsRepo
    {
        Task<PurchaseOrderLineItems> GetById(int id);
        Task<List<PurchaseOrderLineItems>> GetAll(Int32 Limit);
        Task<String> Create(PurchaseOrderLineItems NewPoLineItem);
        Task<Boolean> Update(PurchaseOrderLineItems PoLineItemToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
    }

    public class PurchaseOrdersLineItemsRepo : IPurchaseOrdersLineItemsRepo
    {
        private readonly IConfiguration _config;
        private string TableName = "purchase_order_line_items";
        private string selectlimit = "30";
        public PurchaseOrdersLineItemsRepo(IConfiguration config)
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
        Action: Insert purchase order line item data to database
        Input: new Purchase Orders line item data
        output: string of PurchaseOrders line item id
        */
        public async Task<string> Create(PurchaseOrderLineItems NewPoLineItem)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<PurchaseOrderLineItems>(NewPoLineItem);
                return result.ToString();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get All purchase order line item data from database
        Input: int limit
        output: list of PurchaseOrders line item
        */
        public async Task<List<PurchaseOrderLineItems>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<PurchaseOrderLineItems> result = new List<PurchaseOrderLineItems>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<PurchaseOrderLineItems>("Select * from " + TableName + " Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<PurchaseOrderLineItems>();
                }
                return result.ToList();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order line item data by id from database
        Input: int id
        output: list of PurchaseOrders line item
        */
        public async Task<PurchaseOrderLineItems> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<PurchaseOrderLineItems>(id);
                return result;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update purchase order line item data by id from database
        Input: purchase order line item data
        output: Boolean
        */
        public async Task<Boolean> Update(PurchaseOrderLineItems PoLineItemToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<PurchaseOrderLineItems>(PoLineItemToUpdate);
                return result;
            }

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Update Specific purchase order line item data by id from database
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
