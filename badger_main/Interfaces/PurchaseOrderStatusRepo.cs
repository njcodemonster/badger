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

namespace badgerApi.Interfaces
{
    public interface IPurchaseOrderStatusRepository
    {
        Task<List<PurchaseOrderStatus>> GetByName(string name);
        Task<PurchaseOrderStatus> GetById(int id);
        Task<List<PurchaseOrderStatus>> GetAll(Int32 Limit);

    }
    public class PurchaseOrderStatusRepo : IPurchaseOrderStatusRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "purchase_order_status";
        public PurchaseOrderStatusRepo(IConfiguration config)
        {
            _config = config;
        }
        public IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(_config.GetConnectionString("ProductsDatabase"));
            }
        }

        public async Task<string> Create(PurchaseOrderStatus NewPurchaseOrderStatus)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<PurchaseOrderStatus>(NewPurchaseOrderStatus);
                return result.ToString();
            }
        }

        public async Task<List<PurchaseOrderStatus>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<PurchaseOrderStatus> result = new List<PurchaseOrderStatus>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<PurchaseOrderStatus>("Select * from purchase_order_status Limit 30;");
                }
                else
                {
                    result = await conn.GetAllAsync<PurchaseOrderStatus>();
                }
                return result.ToList();
            }
        }

        public async Task<PurchaseOrderStatus> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<PurchaseOrderStatus>(id);
                return result;
            }
        }

        public async Task<List<PurchaseOrderStatus>> GetByName(string name)
        {
            try
            {
                string QueryWhereClause;
                QueryWhereClause = " where po_status_name = '"+name+"'";
                
                using (IDbConnection conn = Connection)
                {
                    string sQuery = "SELECT * from " + TableName +" "+ QueryWhereClause;
                    conn.Open();
                    var result = conn.Query<PurchaseOrderStatus>(sQuery);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


    }
}
