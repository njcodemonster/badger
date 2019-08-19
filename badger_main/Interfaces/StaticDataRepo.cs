using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using badgerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using CommonHelper;

namespace badgerApi.Interfaces
{
    public interface IStaticDataRepo
    {
        Task<List<Attributes>> GetByTypeId(int attribute_type_id);
    }

  
    public class StaticDataRepo
    {

        private readonly IConfiguration _config;
        private string TableName = "sku";
        private string selectlimit = "30";
        public StaticDataRepo(IConfiguration config)
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



        /*
        Developer: Ubaid
        Date: 17-8-19 
        Action: getting attributes with type id from database
        Input: type id
        output: attributes list
     */
        public async Task<List<Attributes>> GetByTypeId(Int32 attribute_type_id)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Attributes> result = new List<Attributes>();

                result = await conn.QueryAsync<Attributes>("Select * from " + TableName + " Where attribute_type_id=" + attribute_type_id.ToString() + ";");


                return result.ToList();
            }
        }
    }
}
