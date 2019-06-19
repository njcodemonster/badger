using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using badgerApi.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using Dapper.Contrib;

namespace badgerApi.Interfaces
{
    public interface IUserEventsRepo
    {
        Task<bool> AddUserEventAsync(int eventtype, int reffrenceId, string description, int userID, double createdat);
    }

    public class UserEventsRepo : IUserEventsRepo
    {
        private readonly IConfiguration _config;
        private string TableName = "user_events";
        private string selectlimit = "30";
        public UserEventsRepo(IConfiguration config)
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

        public async Task<bool> AddUserEventAsync(int eventtype, int reffrenceId, string description, int userID, double createdat)
        {
            Boolean res = false;
            try
            {
                using (IDbConnection conn = Connection)
                {

                    String DInsertQuery = "insert into " + TableName + " values (null," + eventtype.ToString() + "," + reffrenceId.ToString() + ",\"" + description.ToString() + "\"," + userID.ToString() + "," + createdat.ToString() + ")";
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
