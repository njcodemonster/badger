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
        Task<List<Items>> GetAll(Int32 Limit);
        Task<List<Items>> GetItemById(string id);
        Task<List<Items>> GetItemByBarcode(string Barcode);
        Task<List<Items>> GetItemByBagNumber(string BagNumber);
        Task<List<Items>> GetBySkuFamily(string BagNumber, int Limit);
        Task<List<Items>> GetByProductId(string ProductId, int Limit);
        Task<List<Items>> GetByVendorId(string VendorId, int Limit);
        Task<List<Items>> GetBySkuId(string SkuId, int Limit);
        Task<List<Items>> GetByStatusId(string StatusId, int Limit);
        Task<List<Items>> GetByPublishDate(string PublishDate, int Limit);
        Task<List<Items>> GetByRaStatus(string RaStatusId, int Limit);
        Task<List<Items>> GetByPublishDateRange(string StartDate, string EndDate, int Limit);
        Task<List<Items>> GetByPublishedBy(string PublishedBy, int Limit);
        Task<List<Items>> GetByUpdateDateRange(string StartDate, string EndDate, int Limit);
        Task<List<Items>> GetByCreateDateRange(string StartDate, string EndDate, int Limit);
        Task<List<Items>> GetAfterDate(string AfterDate, int Limit);
        Task<List<Items>> GetBeforeDate(string BeforeDate, int Limit);
        Task<String> Create(Items NewItem);
        Task<Boolean> Update(Items ItemToUpdate);
        Task UpdateSpeific(Dictionary<String, String> ValuePairs, String where);

    }
    public class ItemRepo : ItemRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "items";
        private string selectlimit = "30";

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

        public async Task<List<Items>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Items> result = new List<Items>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<Items>("Select * from " + TableName + " Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<Items>();
                }
                return result.ToList();
            }
        }

        public async Task<List<Items>> GetItemById(string id)
        {
            try
            {
                    List<Items> ToRetrun = new List<Items>();

                    int countComma = id.Count(c=> c== ',');
                    string QueryWhereClause;
                    if (countComma > 0)
                    {
                        QueryWhereClause = " where item_id IN (" + id + ") ";
                        string Query = "SELECT * from items " + QueryWhereClause;
                        using (IDbConnection conn = Connection)
                        {
                            var result = conn.Query<Items>(Query);
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

        public async Task<List<Items>> GetItemByBarcode(string Barcode)
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
                    string Query = "SELECT * from items " + QueryWhereClause; 
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Items>> GetItemByBagNumber(string BagNumber)
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
                    string Query = "SELECT * from items " + QueryWhereClause; 
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Items>> GetBySkuFamily(string SkuFamily, int Limit)
        {
            try
            {
                string QueryWhereClause;
                string LimitQuery = "" ;
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

                if (Limit > 0 ) {
                    LimitQuery = " Limit  "+Limit.ToString();
                }

                string Query = "SELECT * from items " + QueryWhereClause + LimitQuery;

                using (IDbConnection conn = Connection)
                { 
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            } 

        }

        public async Task<List<Items>> GetByProductId(string ProductId, int Limit)
        {
            try
            {
                string LimitQuery;
                string QueryWhereClause;
                int countComma = ProductId.Count(c => c == ',');
                if (countComma > 0)
                {
                    QueryWhereClause = " where product_id IN (" + ProductId + ")";
                }
                else
                {
                    QueryWhereClause = " where product_id = " + ProductId;
                }

                if (Limit > 0)
                {
                    LimitQuery = " Limit  " + Limit.ToString();
                }
                else
                {
                    LimitQuery = "";
                }

                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause + LimitQuery;
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Items>> GetByStatusId(string StatusId, int Limit)
        {
            try
            {
                string LimitQuery;
                string QueryWhereClause;
                int countComma = StatusId.Count(c => c == ',');
                if (countComma > 0)
                {
                    QueryWhereClause = " where item_status_id IN (" + StatusId + ")";
                }
                else
                {
                    QueryWhereClause = " where item_status_id = " + StatusId;
                }

                if (Limit > 0)
                {
                    LimitQuery = " Limit  " + Limit.ToString();
                }
                else
                {
                    LimitQuery = "";
                }

                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause + LimitQuery;
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Items>> GetByVendorId(string VendorId, int Limit)
        {
            try
            {
                string LimitQuery;
                string QueryWhereClause;
                int countComma = VendorId.Count(c => c == ',');
                if (countComma > 0)
                {
                    QueryWhereClause = " where vendor_id IN (" + VendorId + ")";
                }
                else
                {
                    QueryWhereClause = " where vendor_id = " + VendorId;
                }

                if (Limit > 0)
                {
                    LimitQuery = " Limit  " + Limit.ToString();
                }
                else
                {
                    LimitQuery = "";
                }

                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause + LimitQuery;
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Items>> GetBySkuId(string SkuId, int Limit)
        {
            try
            {
                string LimitQuery;
                string QueryWhereClause;
                int countComma = SkuId.Count(c => c == ',');
                if (countComma > 0)
                {
                    QueryWhereClause = " where sku_id IN (" + SkuId + ")";
                }
                else
                {
                    QueryWhereClause = " where sku_id = " + SkuId;
                }

                if (Limit > 0)
                {
                    LimitQuery = " Limit  " + Limit.ToString();
                }
                else
                {
                    LimitQuery = "";
                }

                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause + LimitQuery;
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<List<Items>> GetByPublishDate(string PublishDate, int Limit)
        {
            try
            {
                string LimitQuery;
                string QueryWhereClause;
                int countComma = PublishDate.Count(c => c == ',');
                if (countComma > 0)
                {
                    QueryWhereClause = " where published IN (" + PublishDate + ")";
                }
                else
                {
                    QueryWhereClause = " where published = " + PublishDate;
                }

                if (Limit > 0)
                {
                    LimitQuery = " Limit  " + Limit.ToString();
                }
                else
                {
                    LimitQuery = "";
                }

                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause + LimitQuery;
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Items>> GetByPublishDateRange(string StartDate, string EndDate, int Limit)
        {
            try
            {
                string LimitQuery;
                string QueryWhereClause;
                QueryWhereClause = " where published >= " + StartDate + " AND published <= " + EndDate;

                if (Limit > 0)
                {
                    LimitQuery = " Limit  " + Limit.ToString();
                }
                else
                {
                    LimitQuery = "";
                }

                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause + LimitQuery;
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Items>> GetByRaStatus(string RaStatusId, int Limit)
        {
            try
            {
                string LimitQuery;
                string QueryWhereClause;
                int countComma = RaStatusId.Count(c => c == ',');
                if (countComma > 0)
                {
                    QueryWhereClause = " where ra_status IN (" + RaStatusId + ")";
                }
                else
                {
                    QueryWhereClause = " where ra_status = " + RaStatusId;
                }

                if (Limit > 0)
                {
                    LimitQuery = " Limit  " + Limit.ToString();
                }
                else
                {
                    LimitQuery = "";
                }

                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause + LimitQuery;
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Items>> GetByPublishedBy(string PublishedBy, int Limit)
        {
            try
            {
                string LimitQuery;
                string QueryWhereClause;
                int countComma = PublishedBy.Count(c => c == ',');
                if (countComma > 0)
                {
                    QueryWhereClause = " where published_by IN (" + PublishedBy + ")";
                }
                else
                {
                    QueryWhereClause = " where published_by = " + PublishedBy;
                }

                if (Limit > 0)
                {
                    LimitQuery = " Limit  " + Limit.ToString();
                }
                else
                {
                    LimitQuery = "";
                }

                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause + LimitQuery;
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Items>> GetByUpdateDateRange(string StartDate, string EndDate, int Limit)
        {
            try
            {
                string LimitQuery;
                string QueryWhereClause;
                QueryWhereClause = " where updated_at >= " + StartDate + " AND updated_at <= " + EndDate;

                if (Limit > 0)
                {
                    LimitQuery = " Limit  " + Limit.ToString();
                }
                else
                {
                    LimitQuery = "";
                }

                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause + LimitQuery;
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<List<Items>> GetByCreateDateRange(string StartDate, string EndDate, int Limit)
        {
            try
            {
                string LimitQuery;
                string QueryWhereClause;
                QueryWhereClause = " where created_at >= " + StartDate + " AND created_at <= " + EndDate;

                if (Limit > 0)
                {
                    LimitQuery = " Limit  " + Limit.ToString();
                }
                else
                {
                    LimitQuery = "";
                }

                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause + LimitQuery;
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Items>> GetAfterDate(string AfterDate, int Limit)
        {
            try
            {
                string LimitQuery;
                string QueryWhereClause;
                QueryWhereClause = " where created_at > " + AfterDate;

                if (Limit > 0)
                {
                    LimitQuery = " Limit  " + Limit.ToString();
                }
                else
                {
                    LimitQuery = "";
                }

                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause + LimitQuery;
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Items>> GetBeforeDate(string BeforeDate, int Limit)
        {
            try
            {
                string LimitQuery;
                string QueryWhereClause;
                QueryWhereClause = " where created_at > " + BeforeDate;

                if (Limit > 0)
                {
                    LimitQuery = " Limit  " + Limit.ToString();
                }
                else
                {
                    LimitQuery = "";
                }

                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause + LimitQuery;
                    var result = conn.Query<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> Create(Items NewItem)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Items>(NewItem);
                return result.ToString();
            }
        }

        public async Task<Boolean> Update(Items ItemToUpdate)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Items>(ItemToUpdate);
                return result;
            }

        }

       
        public async Task UpdateSpeific(Dictionary<String, String> ValuePairs, String where)
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
