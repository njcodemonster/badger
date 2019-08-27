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

namespace itemService.Interfaces
{
    public interface IItemEventsRepository
    {
        Task<List<ItemEvents>> GetItemHistoryAll(int item_id,string StartDate, string EndDate, int Limit);
      //  Task<List<ItemEvents>> GetItemHistoryByItemId(int item_id, string StartDate, string EndDate, int Limit);
        //Task<List<ItemEvents>> GetItemHistoryByBarcode(int Barcode, string StartDate, string EndDate, int Limit);
        //Task<List<ItemEvents>> GetItemHistoryByEventType(string Event_type_id, string StartDate, string EndDate, int Limit);
        //Task<List<ItemEvents>> GetItemHistoryByRefId(string id, string StartDate, string EndDate, int Limit);


        //item/GetItemHistory
        //item/GetItemHistory
        //item/GetItemHistoryByBarcode
        //item/GetItemHistoryByEventType
        //item/GetItemHistoryByRefId


    }
    public class ItemEventsRepo : IItemEventsRepository
    {
        private readonly IConfiguration _config;

        public ItemEventsRepo(IConfiguration config)
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
         * (There is problem in function arguments are not used in function)
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get All Items Events from database
        Input: int item_id,string StartDate, string EndDate, int Limit (There is problem in function arguments are not used in function)
        output: List of item events
        */
        public async Task<List<ItemEvents>> GetItemHistoryAll(int item_id,string StartDate, string EndDate, int Limit)
        {
            try
            {
                    List<ItemEvents> ToRetrun = new List<ItemEvents>();

                        using (IDbConnection conn = Connection)
                        {
                            var result = await conn.GetAllAsync<ItemEvents>();
                           // ToRetrun.Add(result);
                        }
                          
                return ToRetrun;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /*public async Task<List<ItemEvents>> GetItemHistoryById(int item_id, string StartDate, string EndDate, int Limit)
        {
            try
            {
                string LimitQuery;
                string QueryWhereClause;
                QueryWhereClause = " where  item_id = "+item_id+" published >= " + StartDate + " AND published <= " + EndDate;

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
        public async Task<List<ItemEvents>> GetItemHistoryByBarcode(int id, string StartDate, string EndDate, int Limit)
        {
            try
            {
                List<ItemEvents> ToRetrun = new List<ItemEvents>();


                int itemId = Convert.ToInt32(id);
                using (IDbConnection conn = Connection)
                {
                    var result = conn.Get<ItemEvents>(itemId);
                    ToRetrun.Add(result);
                }

                return ToRetrun;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<ItemEvents>> GetItemHistoryByEventType(string event_type, string StartDate, string EndDate, int Limit)
        {
            try
            {
                List<ItemEvents> ToRetrun = new List<ItemEvents>();


                int itemId = Convert.ToInt32(event_type);
                using (IDbConnection conn = Connection)
                {
                    var result = conn.Get<ItemEvents>(itemId);
                    ToRetrun.Add(result);
                }

                return ToRetrun;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<ItemEvents>> GetItemHistoryByRefId(string id, string StartDate, string EndDate, int Limit)
        {
            try
            {
                List<ItemEvents> ToRetrun = new List<ItemEvents>();


                int itemId = Convert.ToInt32(id);
                using (IDbConnection conn = Connection)
                {
                    var result = conn.Get<ItemEvents>(itemId);
                    ToRetrun.Add(result);
                }

                return ToRetrun;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        */

    }
}
