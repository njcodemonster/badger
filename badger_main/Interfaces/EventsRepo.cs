using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace badgerApi.Interfaces
{
    public interface IEventRepo
    {
        Task<bool> AddEventAsync(int eventtype, int reffrenceId, int userID, string description, double createdat, string tableName);
        Task<bool> AddPurchaseOrdersEventAsync(int po_id, int event_type_id, int reffrence_id, string description, int userID, double createdat, string tableName);
        Task<bool> AddPhotoshootAsync(int productId, int eventTypeId, int reffrenceId, string eventNotes, int userId, double createdAt, string tableName);
        Task<bool> AddVendorEventAsync(int vendor_id,int eventtype, int reffrenceId, int userID, string description, double createdat, string tableName);
        Task<bool> AddItemEventAsync(int item_id, int barcode, int event_type_id, int reffrence_id, string description, int userID, double createdat, string tableName);
    }
    public class EventsRepo : IEventRepo
    {
        private readonly IConfiguration _config;
        public EventsRepo(IConfiguration config)
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

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: dynamic Create Events data by table 
        Input: int eventtype, int reffrenceId,int userID,string description,double createdat , string tableName
        output: boolean
         */
        public async Task<bool> AddEventAsync(int eventtype, int reffrenceId,int userID,string description,double createdat , string tableName)
        {
            Boolean res = false;
            try
            {
                using (IDbConnection conn = Connection)
                {

                    String DInsertQuery = "insert into " + tableName + " values (null,"+ eventtype.ToString() + "," + userID.ToString() + ","+reffrenceId.ToString()+",\"" + description.ToString() + "\"," + createdat.ToString() + ")";
                    var vendorDetails = await conn.QueryAsync<object>(DInsertQuery);
                    res = true;
                }
            }
            catch(Exception ex)
            {

            }
            return res;
        }


        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Create Purchase Orders Events data
        Input: int po_id, int event_type_id, int reffrence_id, string description, int userID, double createdat, string tableName
        output: boolean
         */
        public async Task<bool> AddPurchaseOrdersEventAsync(int po_id, int event_type_id, int reffrence_id, string description, int userID, double createdat, string tableName)
        {
            Boolean res = false;
            try
            {
                using (IDbConnection conn = Connection)
                {
                    String DInsertQuery = "insert into " + tableName + " values (null," + po_id.ToString() + "," + event_type_id.ToString() + "," + reffrence_id.ToString() + ",\"" + description.ToString() + "\"," + userID.ToString() + "," + createdat.ToString() + ")";
                    var vendorDetails = await conn.QueryAsync<object>(DInsertQuery);
                    res = true;
                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Create Vendor Event data
        Input: int vendor_id, int eventtype, int reffrenceId, int userID, string description, double createdat, string tableName
        output: boolean
        */
        public async Task<bool> AddVendorEventAsync(int vendor_id, int eventtype, int reffrenceId, int userID, string description, double createdat, string tableName)
        {
            Boolean res = false;
            try
            {
                using (IDbConnection conn = Connection)
                {

                    String DInsertQuery = "insert into " + tableName + " values (null,"+ vendor_id.ToString()+ "," + eventtype.ToString() + "," + userID.ToString() + "," + reffrenceId.ToString() + ",\"" + description.ToString() + "\"," + createdat.ToString() + ")";
                    var vendorDetails = await conn.QueryAsync<object>(DInsertQuery);
                    res = true;
                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Create Photoshoot Event data
        Input: int productId, int eventTypeId, int reffrenceId, string eventNotes, int userId, double createdAt, string tableName
        output: boolean
        */
        public async Task<bool> AddPhotoshootAsync(int productId, int eventTypeId, int reffrenceId, string eventNotes, int userId, double createdAt, string tableName)
        {
            Boolean res = false;
            try
            {
                using (IDbConnection conn = Connection)
                {

                    String DInsertQuery = "insert into " + tableName + " (`product_id`, `event_type_id`, `reference_id`, `event_notes`, `user_id`, `created_at`) values ("+ productId.ToString() + "," + eventTypeId.ToString() + "," + reffrenceId.ToString() + ",\"" + eventNotes.ToString() + "\",\"" + userId.ToString() + "\"," + createdAt.ToString() + ")";
                    var vendorDetails = await conn.QueryAsync<object>(DInsertQuery);
                    res = true;
                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Create Item Event data
        Input: int item_id, int barcode, int event_type_id, int reffrence_id, string description, int userID, double createdat, string tableName
        output: boolean
        */
        public async Task<bool> AddItemEventAsync(int item_id, int barcode, int event_type_id, int reffrence_id, string description, int userID, double createdat, string tableName)
        {
            Boolean res = false;
            try
            {
                using (IDbConnection conn = Connection)
                {
                    String DInsertQuery = "insert into " + tableName + " values (null," + item_id.ToString() + ","+ barcode.ToString() + "," + event_type_id.ToString() + "," + reffrence_id.ToString() + ",\"" + description.ToString() + "\"," + userID.ToString() + "," + createdat.ToString() + ")";
                    var vendorDetails = await conn.QueryAsync<object>(DInsertQuery);
                    res = true;
                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }
    }
}
