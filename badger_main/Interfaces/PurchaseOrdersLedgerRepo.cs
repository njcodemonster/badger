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
    public interface IPurchaseOrdersLedgerRepository
    {
        Task<PurchaseOrderLedger> GetById(int id);
        Task<List<PurchaseOrderLedger>> GetAll(Int32 Limit);
        Task<String> Create(PurchaseOrderLedger NewPurchaseOrder);
        Task<Boolean> Update(PurchaseOrderLedger PurchaseOrdersToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task<string> Count();
        Task<object> GetPurchaseOrdersLedger(int id);
    }

    public class PurchaseOrdersLedgerRepo : IPurchaseOrdersLedgerRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "purchase_order_ledger";
        private string selectlimit = "30";
        public PurchaseOrdersLedgerRepo(IConfiguration config)
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

        public async Task<string> Create(PurchaseOrderLedger NewPurchaseOrder)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<PurchaseOrderLedger>(NewPurchaseOrder);
                return result.ToString();
            }
        }

        public async Task<string> Count()
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync<String>("select count(transaction_id) from " + TableName + ";");
                return result.FirstOrDefault();
            }
        }

        public async Task<object> GetPurchaseOrdersLedger(int id)
        {
            dynamic poLedger = new ExpandoObject();
            string sQuery = "";
            
            sQuery = "SELECT * from "+ TableName + " WHERE po_id= "+ id.ToString() +" LIMIT 1;";
            
            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> purchaseOrdersLedger = await conn.QueryAsync<object>(sQuery);
                poLedger = purchaseOrdersLedger;
            }
            return poLedger;

        }

        public async Task<List<PurchaseOrderLedger>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<PurchaseOrderLedger> result = new List<PurchaseOrderLedger>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<PurchaseOrderLedger>("Select * from " + TableName + " Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<PurchaseOrderLedger>();
                }
                return result.ToList();
            }
        }



        public async Task<PurchaseOrderLedger> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<PurchaseOrderLedger>(id);
                return result;
            }
        }

        public async Task<Boolean> Update(PurchaseOrderLedger PurchaseOrdersToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<PurchaseOrderLedger>(PurchaseOrdersToUpdate);
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
