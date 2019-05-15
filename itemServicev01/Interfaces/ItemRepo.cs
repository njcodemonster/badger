using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using itemService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using CommonHelper;

namespace itemService.Interfaces
{
    public interface ItemRepository
    {
        Task<List<Items>> getItemById(string id);
        Task<List<Items>> getItemByBarcode(string Barcode);
        Task<List<Items>> getItemByBagNumber(string BagNumber);
        Task<List<Items>> GetBySkuFamily(string BagNumber);



    }
    public class ItemRepo : ItemRepository
    {
        private readonly IConfiguration _config;

        public ItemRepo(IConfiguration config)
        {
            _config = config;
        }
        public IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(_config.GetConnectionString("ItemsDatabase"));
            }
        }

        public Task<List<Items>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Items>> getItemById(string id)
        {
            try
            {
                    List<Items> ToRetrun = new List<Items>();

                    int countComma = id.Count(c=> c== ',');
                    string QueryWhereClause;
                    if (countComma > 0)
                    {
                        QueryWhereClause = " where item_id IN (" + id + ") ";
                        string sQuery = "SELECT * from items " + QueryWhereClause;
                        using (IDbConnection conn = Connection)
                        {
                            var result = conn.Query<Items>(sQuery);
                            ToRetrun = result.ToList();
                        }
                    }
                    else
                    {
                        int itemId = Convert.ToInt32(id);
                        using (IDbConnection conn = Connection)
                        {
                            var result = conn.Get<Items>(itemId);
                            ToRetrun.Add(result);
                        }
                    }
                    return ToRetrun;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Items>> getItemByBarcode(string Barcode)
        {
            try
            {
                string QueryWhereClause;
                int countComma = Barcode.Count(c => c == ',');
                if (countComma > 0)
                {
                    QueryWhereClause = " where barcode IN (" + Barcode + ")";
                }
                else
                {
                    QueryWhereClause = " where barcode = " + Barcode;
                }

                using (IDbConnection conn = Connection)
                {   
                    string sQuery = "SELECT * from items " + QueryWhereClause; 
                    var result = conn.Query<Items>(sQuery);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Items>> getItemByBagNumber(string BagNumber)
        {
            try
            {
                string QueryWhereClause;
                int countComma = BagNumber.Count(c => c == ',');
                if (countComma > 0)
                {
                    QueryWhereClause = " where bag_code IN (" + BagNumber + ")";
                }
                else
                {
                    QueryWhereClause = " where bag_code = " + BagNumber;
                }

                using (IDbConnection conn = Connection)
                {
                    string sQuery = "SELECT * from items " + QueryWhereClause; 
                    var result = conn.Query<Items>(sQuery);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Items>> GetBySkuFamily(string SkuFamily)
        {
            try
            {
                string QueryWhereClause;
                int countComma = SkuFamily.Count(c => c == ',');
                if (countComma > 0)
                {
                    string InString = SkuFamily;
                    InString = InString.Replace(",", "\",\"");
                    QueryWhereClause = " where sku IN (\"" + InString + "\")";
                }
                else
                {
                    QueryWhereClause = " where sku = \"" + SkuFamily+ "\"";
                }

                using (IDbConnection conn = Connection)
                {
                    string sQuery = "SELECT * from items " + QueryWhereClause; 
                    var result = conn.Query<Items>(sQuery);
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
