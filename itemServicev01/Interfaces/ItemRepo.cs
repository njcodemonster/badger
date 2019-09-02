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
using System.Dynamic;

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
        Task<List<Items>> GetByPOid(int PO_id);
        Task<String> GetitemCountBySkuStatus(string po_id, string sku, string item_status_id);
        Task<String> Create(Items NewItem);
        Task<Boolean> Update(Items ItemToUpdate);
        Task UpdateSpeific(Dictionary<String, String> ValuePairs, String where);
        Task SetProductItemSentToPhotoshoot(string product_id);
        Task<List<Items>> CheckBarcodeExist(int barcode);
        Task<String> SetProductItemForPhotoshoot(int skuId, int status);
        Task<List<Items>> GetItemGroupByProductId(int PO_id);
        Task<Object> GetBarcode(int barcode);
        Task<bool> DeleteItemByProduct(string id);
        Task<bool> DeleteBySku(Items NewItem, int qty);
    }
    public class ItemRepo : ItemRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "items";
        private string selectlimit = "30";

        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();

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

        /*
       Developer: Sajid Khan
       Date: 7-13-19 
       Action: Get list of items from database
       Input: int limit
       output: List of items
       */
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
        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get list of items from database
        Input: int limit
        output: List of items
        */
        public async Task<String> GetitemCountBySkuStatus(string po_id, string sku, string item_status_id)
        {
            using (IDbConnection conn = Connection)
            {
                string query = "Select Count(item_id) from items where item_status_id=" + item_status_id + " and  po_id=" + po_id + " and sku='" + sku + "'";
                var result = await conn.QueryAsync<string>(query);

                return result.FirstOrDefault();

            }
        }


        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by id also with comma multiple ids or without comma seperate single to database
        Input: string id
        output: List of items
        */
        public async Task<List<Items>> GetItemById(string id)
        {
            try
            {
                List<Items> ToRetrun = new List<Items>();

                int countComma = id.Count(c => c == ',');
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by PO_id to database
        Input: int PO_id
        output: List of items
        */
        public async Task<List<Items>> GetByPOid(int PO_id)
        {
            try
            {
                string QueryWhereClause = "where item_status_id <> 5 AND PO_id=" + PO_id.ToString();
                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause;
                    var result = await conn.QueryAsync<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by PO_id to database
        Input: int PO_id
        output: List of items
        */
        public async Task<List<Items>> GetItemGroupByProductId(int PO_id)
        {
            try
            {
                string QueryWhereClause = "where item_status_id <> 5 AND PO_id=" + PO_id.ToString() + " GROUP BY product_id";
                using (IDbConnection conn = Connection)
                {
                    string Query = "SELECT * from items " + QueryWhereClause;
                    var result = await conn.QueryAsync<Items>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by Barcode to database
        Input: string Barcode
        output: List of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by bag_code to database
        Input: string bag_code
        output: List of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by SkuFamily and limit to database
        Input: string SkuFamily, int Limit
        output: List of items
        */
        public async Task<List<Items>> GetBySkuFamily(string SkuFamily, int Limit)
        {
            try
            {
                string QueryWhereClause;
                string LimitQuery = "";
                int countComma = SkuFamily.Count(c => c == ',');
                if (countComma > 0)
                {
                    string InString = SkuFamily;
                    InString = InString.Replace(",", "\",\"");
                    QueryWhereClause = " where sku IN (\"" + InString + "\")";
                }
                else
                {
                    QueryWhereClause = " where sku = \"" + SkuFamily + "\"";
                }

                if (Limit > 0)
                {
                    LimitQuery = " Limit  " + Limit.ToString();
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by ProductId and limit to database
        Input: string ProductId, int Limit
        output: List of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by item_status_id and limit to database
        Input: string StatusId, int Limit
        output: List of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by VendorId and limit to database
        Input: string VendorId, int Limit
        output: List of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by SkuId and limit to database
        Input: string SkuId, int Limit
        output: List of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by published date and limit to database
        Input: string PublishDate, int Limit
        output: List of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by published range start,end and limit to database
        Input: string StartDate, string EndDate, int Limit
        output: List of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by RaStatus and limit to database
        Input: string RaStatusId, int Limit
        output: List of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by Published By and limit to database
        Input: string PublishedBy, int Limit
        output: List of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by updated at in range start date,end date and limit to database
        Input: string StartDate, string EndDate, int Limit
        output: List of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by created at in range start date,end date and limit to database
        Input: string StartDate, string EndDate, int Limit
        output: List of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by created at after date and limit to database
        Input: string AfterDate, int Limit
        output: List of items
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get items by created at  before date and limit to database
        Input: string BeforeDate, int Limit
        output: List of items
        */
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
        /*Developer: ubaid
       Date:5-7-19
       Action:get NewItem Model from controller and insert the NewItem
       Input: NewItem Model 
       output: New Item id
       */
        public async Task<string> Create(Items NewItem)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Items>(NewItem);
                return result.ToString();
            }
        }

        /*Developer: Hamza Haq
        Date:31-8-19
        Action:get Item Model from controller with qty to Delete
        Input: ItemToDelete Model 
        output: boolean 
        */
        public async Task<bool> DeleteBySku(Items ItemToDelete, int qty)
        {
            bool IsSuccess = false;

            try
            {
                using (IDbConnection conn = Connection)
                {
                    string sQuery = "Delete from items where sku='" + ItemToDelete.sku + "' and po_id='" + ItemToDelete.PO_id + "' limit " + qty + ";";
                    var result = await conn.QueryAsync<string>(sQuery);

                    IsSuccess = true;
                }
            }
            catch (Exception)
            {

                IsSuccess = false;
            }

            return IsSuccess;
        }
        /*
         * (There is mistake in function,There is no product id used in function )
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get count of phootoshots where id equal to zero from database
        Input: string product_id (There is mistake in function,There is no product id used in function )
        output: boolean
        */
        public async Task SetProductItemSentToPhotoshoot(string product_id)
        {
            using (IDbConnection conn = Connection)
            {
                await conn.QueryAsync<String>("select count(photoshoot_id) from " + TableName + " where photoshoot_id = 0;");
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: update item by id to database
        Input: item data
        output: boolean
        */
        public async Task<Boolean> Update(Items ItemToUpdate)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Items>(ItemToUpdate);
                return result;
            }

        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: update specific item by id to database
        Input: Dictionary<String, String> ValuePairs, String where condition
        output: boolean
        */
        public async Task UpdateSpeific(Dictionary<String, String> ValuePairs, String where)
        {
            QueryHelper qHellper = new QueryHelper();
            string UpdateQuery = qHellper.MakeUpdateQuery(ValuePairs, TableName, where);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync(UpdateQuery);

            }

        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: Get item status by id and update item status to database
        Input: int skuId, int status
        output: string
        */
        public async Task<String> SetProductItemForPhotoshoot(int skuId, int status)
        {

            string ToReturn = "success";

            IEnumerable<Items> item = new List<Items>();
            string itemSelectQuery = "";
            string itemUpdateQuery = "";

            if (status == 1)
            {
                itemSelectQuery = "SELECT * FROM items where items.sku_id= " + skuId.ToString() + " ORDER BY RAND() LIMIT 1;";
            }
            else if (status == 0)
            {
                itemSelectQuery = "SELECT * FROM items where items.sku_id= " + skuId.ToString() + " AND item_status_id = 6;";
            }

            using (IDbConnection conn = Connection)
            {
                item = await conn.QueryAsync<Items>(itemSelectQuery);
            }

            if (item.Count() > 0)
            {
                if (status == 1)
                {
                    itemUpdateQuery = "update items set  item_status_id = 6, updated_at = " + _common.GetTimeStemp() + " where item_id = " + item.First().item_id.ToString() + "; ";
                }
                else if (status == 0)
                {
                    itemUpdateQuery = "update items set  item_status_id = 1, updated_at = " + _common.GetTimeStemp() + " where item_id = " + item.First().item_id.ToString() + "; ";
                }
                using (IDbConnection conn = Connection)
                {
                    await conn.QueryAsync<Items>(itemUpdateQuery);
                }
            }
            else
            {
                ToReturn = "failed";
            }

            return ToReturn;
        }

        /*
        Developer: Sajid Khan
        Date: 7-20-19 
        Action: Check Barcode already Exist by barcode data from database
        Input: int barcode
        output: dynamic list of barcode
        */
        public async Task<List<Items>> CheckBarcodeExist(int barcode)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Items> result = new List<Items>();

                string squery = "Select * from " + TableName + " WHERE barcode = '" + barcode + "';";

                result = await conn.QueryAsync<Items>(squery);

                return result.ToList();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 08-09-19 
        Action: Get barcode by barcode from database 
        Input:  int barcode
        output: dynamic list of barcode data
        */
        public async Task<Object> GetBarcode(int barcode)
        {
            dynamic barcodeDetails = new ExpandoObject();

            string sQuery = "SELECT item_id as value,barcode as label,'barcode' as type FROM " + TableName + " WHERE barcode =" + barcode + ";";

            using (IDbConnection conn = Connection)
            {
                barcodeDetails = await conn.QueryAsync<object>(sQuery);

            }
            return barcodeDetails;
        }


        /*
        Developer: Rizwan Ali
        Date: 08-09-19 
        Action: delete item from db by product 
        Input:  int product id
        output: bool status
        */
        public async Task<bool> DeleteItemByProduct(string id)
        {
            bool status = false;
            int product_id = int.Parse(id);
            string sQuery = "delete FROM items WHERE product_id=" + product_id;
            try
            {
                using (IDbConnection conn = Connection)
                {
                    var a = await conn.QueryAsync<string>(sQuery);
                    status = true;

                }
            }
            catch (Exception)
            {
                status = false;
            }

            return status;
        }

    }
}
