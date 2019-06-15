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

namespace badgerApi.Models
{
    public partial class LogiDetails
    {
        public String UserIdentity { get; set; }
        public String UserPass { get; set; }
    }
    
}
namespace badgerApi.Interfaces
{
   
    public interface IUserRepo
    {
         Task<Users> AuthenticateUser(LogiDetails logiDetails);
    }

    public class UserRepo : IUserRepo
    {
        private readonly IConfiguration _config;
        private string TableName = "Users";
        private string selectlimit = "30";
        public UserRepo(IConfiguration config)
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

        public async Task<Users> AuthenticateUser(LogiDetails logiDetails)
        {
            IEnumerable<Users> user = new List<Users>();
            Users Dumy = new Users();
            using (IDbConnection conn = Connection)
            {
                user = await conn.QueryAsync<Users>("SELECT users.* FROM users  where users.email = \"" + logiDetails.UserIdentity + "\" and users.password = \"" + logiDetails.UserPass + "\" ; ");
               
            }
            if (user.Count() < 0)
            {
                user.Append(Dumy);
            }
            return user.First();
        }
    }
}
