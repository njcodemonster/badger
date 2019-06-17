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

        public async Task<string> Create(PurchaseOrderDiscounts NewPurchaseOrder)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<PurchaseOrderDiscounts>(NewPurchaseOrder);
                return result.ToString();
            }
        }

        public async Task<string> Count()
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync<String>("select count(po_discount_id) from " + TableName + ";");
                return result.FirstOrDefault();
            }
        }

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

        public async Task<PurchaseOrderDiscounts> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<PurchaseOrderDiscounts>(id);
                return result;
            }
        }

        public async Task<Boolean> Update(PurchaseOrderDiscounts PurchaseOrdersToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<PurchaseOrderDiscounts>(PurchaseOrdersToUpdate);
                return result;
            }

        }
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
