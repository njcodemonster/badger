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
    public interface IPurchaseOrdersRepository
    {
        Task<PurchaseOrders> GetById(int id);
        Task<List<PurchaseOrders>> GetAll(Int32 Limit);
        Task<String> Create(PurchaseOrders NewPurchaseOrder);
        Task<Boolean> Update(PurchaseOrders PurchaseOrdersToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task<string> Count();
        Task<object> GetPurchaseOrdersPageList(int limit);
    }
    public class PurchaseOrdersRepo : IPurchaseOrdersRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "purchase_orders";
        private string selectlimit = "30";
        public PurchaseOrdersRepo(IConfiguration config)
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

        public async Task<string> Create(PurchaseOrders NewPurchaseOrder)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<PurchaseOrders>(NewPurchaseOrder);
                return result.ToString();
            }
        }

        public async Task<string> Count()
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync<String>("select count(po_id) from " + TableName + ";");
                return result.FirstOrDefault();
            }
        }

        public async Task<List<PurchaseOrders>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<PurchaseOrders> result = new List<PurchaseOrders>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<PurchaseOrders>("Select * from " + TableName + " Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<PurchaseOrders>();
                }
                return result.ToList();
            }
        }



        public async Task<PurchaseOrders> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<PurchaseOrders>(id);
                return result;
            }
        }

        public async Task<Boolean> Update(PurchaseOrders PurchaseOrdersToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<PurchaseOrders>(PurchaseOrdersToUpdate);
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

        public async Task<object> GetPurchaseOrdersPageList(int limit)
        {

            dynamic poPageList = new ExpandoObject();
            string sQuery = "";
            if (limit > 0)
            {
                sQuery = "SELECT a.po_id, a.vendor_po_number, a.vendor_invoice_number, a.vendor_order_number, a.vendor_id, a.order_date,b.vendor_name as vendor, a.po_status, a.updated_at FROM purchase_orders a left JOIN(SELECT vendor.vendor_id, vendor.vendor_name FROM vendor GROUP BY vendor.vendor_id) b ON b.vendor_id = a.vendor_id order by a.po_id asc limit " + limit + ";";
            }
            else
            {
                sQuery = "SELECT a.po_id, a.vendor_po_number, a.vendor_invoice_number, a.vendor_order_number, a.vendor_id, a.order_date,b.vendor_name as vendor, a.po_status, a.updated_at FROM purchase_orders a left JOIN(SELECT vendor.vendor_id, vendor.vendor_name FROM vendor GROUP BY vendor.vendor_id) b ON b.vendor_id = a.vendor_id order by a.po_id asc";
            }

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> purchaseOrdersInfo = await conn.QueryAsync<object>(sQuery);
                poPageList.purchaseOrdersInfo = purchaseOrdersInfo;
            }
            return poPageList;

        }
    }
}
