using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GenericModals.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace badgerApi.Interfaces
{
    public interface iBarcodeRangeRepo
    {
        Task<List<Barcode>> GetBarcodeRangeList();
        Task<List<Barcode>> GetBarcodeRangeList(int start,int limit);


    }
    public class BarcodeRangeRepo : iBarcodeRangeRepo
    {
        private readonly IConfiguration _config;
        private string selectlimit = "";
        private string TableName = "barcode_range";
        public BarcodeRangeRepo(IConfiguration config)
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
        public async Task<List<Barcode>> GetBarcodeRangeList()
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Barcode> result = new List<Barcode>();
                result = await conn.QueryAsync<Barcode>("Select * from " + TableName + ";");
               
                return result.ToList();
            }
        }

        public async Task<List<Barcode>> GetBarcodeRangeList(int start, int limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Barcode> result = new List<Barcode>();
                if(limit > 0)
                result = await conn.QueryAsync<Barcode>("Select * from " + TableName + " limit " + start + "," + limit + ";");
                else
                    result = await conn.QueryAsync<Barcode>("Select * from " + TableName + ";");
                return result.ToList();
            }
        }
    }

}
