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
        Task<bool> AddVendorEvents(int eventtype, int userID, int reffrenceId, string description, double createdat, string tableName);
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

        public async Task<bool> AddVendorEvents(int eventtype, int userID, int reffrenceId, string description, double createdat, string tableName)
        {
            Boolean res = false;
            try
            {
                using (IDbConnection conn = Connection)
                {

                    String DInsertQuery = "insert into " + tableName + " values (null," + eventtype.ToString() + "," + userID.ToString() + "," + reffrenceId.ToString() + ",\"" + description.ToString() + "\"," + createdat.ToString() + ")";
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
