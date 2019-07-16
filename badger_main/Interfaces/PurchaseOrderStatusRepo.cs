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
        private string selectlimit = "30";
        public PurchaseOrderStatusRepo(IConfiguration config)
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
        Action: Insert purchase order status to database
        Input: new status data
        output: string of status id
        */
        public async Task<string> Create(PurchaseOrderStatus NewPurchaseOrderStatus)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<PurchaseOrderStatus>(NewPurchaseOrderStatus);
                return result.ToString();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: get all purchase order status from database
        Input: int limit
        output: list of status
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: get purchase order status by id from database
        Input: int id
        output: list of status
        */
        public async Task<PurchaseOrderStatus> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<PurchaseOrderStatus>(id);
                return result;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order status by name from database
        Input: string name
        output: list of status
        */
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
