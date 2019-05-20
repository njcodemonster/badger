
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
    using System.Dynamic;

    namespace badgerApi.Interfaces
    {
        public interface IVendorRepRepository
        {
          
            Task<String> Create(VendorContactPerson VendorRep);
          
        }
        public class VendorRepRepo : IVendorRepRepository
        {
            private readonly IConfiguration _config;
            private string TableName = "vendor";
            private string selectlimit = "30";
            public VendorRepRepo(IConfiguration config)
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

            public async Task<string> Create(VendorContactPerson NewVendorRep)
            {
                using (IDbConnection conn = Connection)
                {
                    var result = await conn.InsertAsync<VendorContactPerson>(NewVendorRep);
                    return result.ToString();
                }
            }
        }
}
