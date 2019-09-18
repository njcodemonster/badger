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

namespace badgerApi.Interfaces
{
    public interface ICalculationsRepository
    {
        Task<Calculations> GetById(int id);
        Task<List<Calculations>> GetAll(Int32 Limit);
        Task<String> Create(Calculations NewCalculations);
        Task<Boolean> Update(Calculations CalculationsToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
    }
    public class CalculationsRepo : ICalculationsRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "calculations";
        private string selectlimit = "30";
        public CalculationsRepo(IConfiguration config)
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
           Developer: Hamza haq
           Date: 17-9-19 
           Action: creating new calculations type to database
           Input: new attribute
           output: attribute id
        */
        public async Task<string> Create(Calculations NewCalculations)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Calculations>(NewCalculations);
                return result.ToString();
            }
        }

        /*
           Developer: Hamza haq
           Date: 17-9-19 
           Action: getting list of calculations from database 
           Input: limit
           output: list of attribute
        */
        public async Task<List<Calculations>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Calculations> result = new List<Calculations>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<Calculations>("Select * from " + TableName + " Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<Calculations>();
                }
                return result.ToList();
            }
        }


        /*
          Developer: Hamza haq
          Date: 17-9-19 
          Action: getting calculations by id from database 
          Input: int id
          output: attribute
       */
        public async Task<Calculations> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<Calculations>(id);
                return result;
            }
        }
        /*
            Developer: Hamza haq
            Date: 17-9-19 
            Action: update Calculations to database
            Input: CalculationsToUpdate
            output: result
        */
        public async Task<Boolean> Update(Calculations CalculationsToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Calculations>(CalculationsToUpdate);
                return result;
            }

        }
        /*
           Developer: Hamza haq
           Date: 17-9-19 
           Action: UpdateSpecific to database
           Input: Dictionary<String, String> ValuePairs, String where
           output: result
       */
        public async Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where)
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
