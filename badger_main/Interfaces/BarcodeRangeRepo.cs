using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GenericModals.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper.Contrib.Extensions;

namespace badgerApi.Interfaces
{
    public interface iBarcodeRangeRepo
    {
        Task<List<Barcode>> GetBarcodeRangeList();
        Task<List<Barcode>> GetBarcodeRangeList(int start,int limit);
        Task<bool> Validate(Barcode barcode);
        Task<bool> DeleteBarcode(int id);
        Task<string> CreateOrUpdate(Barcode barcode);
        Task<bool> ValidateBarcode(string barcode);
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
                if (limit > 0)
                    result = await conn.QueryAsync<Barcode>("Select * from " + TableName + " ORDER BY size DESC limit " + start + "," + limit + ";");
                else
                    result = await conn.QueryAsync<Barcode>("Select * from " + TableName + " ORDER BY size DESC ;");
                return result.ToList();
            }
        }
        /*
        Developer: Rizwan ali
        Date: 7-7-19 
        Action: Insert new Category into database
        Input: Category data
        output: string of Category data
*/
        public async Task<bool> Validate(Barcode barcode)
        {
            bool status = false;
            try
            {
                long result; // = default;

                using (IDbConnection conn = Connection)
                {
                    String selectQuery = String.Empty;
                    if (barcode.id != -1)
                    {
                        //Update Validate Scenario
                        selectQuery = "SELECT  * FROM(SELECT * FROM barcode_range WHERE " + barcode.barcode_from + " BETWEEN barcode_from AND barcode_to OR " + barcode.barcode_to + " BETWEEN barcode_from AND barcode_to) c WHERE c.id <> " + barcode.id + "";
                    }
                    else
                    {
                        //new insertion Validate 
                        selectQuery = "SELECT  * FROM(SELECT * FROM barcode_range WHERE " + barcode.barcode_from + " BETWEEN barcode_from AND barcode_to OR " + barcode.barcode_to + " BETWEEN barcode_from AND barcode_to) c ";
                    }

                    var updateResult = await conn.QueryAsync<object>(selectQuery);
                    if (updateResult.FirstOrDefault() != null)
                    {
                        status = false;
                    }
                    else
                        status = true;

                }

                return status;
            }
            catch (Exception ex)
            {
                return false;
            }


        }
        /*
        Developer: Rizwan ali
        Date: 7-7-19 
        Action: Insert new Category into database
        Input: Category data
        output: string of Category data
        */
        public async Task<string> CreateOrUpdate(Barcode barcode)
        {
            using (IDbConnection conn = Connection)
            {
                if (barcode.id == -1)
                {
                    //Insert
                    string InsertQuery = "insert into barcode_range (size,barcode_from,barcode_to) values ('" + barcode.size + "'," + barcode.barcode_from + "," + barcode.barcode_to + ")";
                    var InsertResult = await conn.QueryAsync<object>(InsertQuery);
                    return "true";
                }
                else
                {
                    //Update
                    string updateQuery = "update barcode_range set size='" + barcode.size + "' , barcode_from = " + barcode.barcode_from + " , barcode_to = " + barcode.barcode_to + " where id= " + barcode.id;
                    var updateResult = await conn.QueryAsync<object>(updateQuery);
                    return "true";
                }

            }
        }

        /*
      Developer: Rizwan ali
      Date: 7-7-19 
      Action: Insert new Category into database
      Input: Category data
      output: string of Category data
*/
        public async Task<bool> DeleteBarcode(int id)
        {
            using (IDbConnection conn = Connection)
            {
                var a = await conn.QueryAsync<object>("delete from barcode_range where id="+id);

                return true;
            }
        }

        public async Task<bool> ValidateBarcode(string barcode)
        {
            using (IDbConnection conn = Connection)
            {
                string query = "SELECT id FROM barcode_range " +
                                $"WHERE {barcode} BETWEEN barcode_from AND barcode_to LIMIT 1;";
               var rangeFound = await conn.QueryFirstOrDefaultAsync<int>(query);
                return rangeFound > 0;
            }
        }
    }

}
