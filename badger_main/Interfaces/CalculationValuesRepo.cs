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
    public interface ICalculationValuesRepository
    {
        Task<CalculationValues> GetById(int id);
        Task<List<CalculationValues>> GetAll(Int32 Limit);
        Task<String> Create(CalculationValues NewCalculationValues);
        Task<Boolean> Update(CalculationValues CalculationValuesToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task<string> CreateProductCalculation(string refference_id, int calculation_id, double value, bool IncreaseQty);
    }
    public class CalculationValuesRepo : ICalculationValuesRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "calculation_values";
        private string selectlimit = "30";
        public CalculationValuesRepo(IConfiguration config)
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
           Action: creating new calculation_Values to database
           Input: new attribute
           output: attribute id
        */
        public async Task<string> Create(CalculationValues NewCalculationValues)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.InsertAsync<CalculationValues>(NewCalculationValues);
                return result.ToString();
            }
        }

        /*
           Developer: Hamza haq
           Date: 17-9-19 
           Action: creating new calculation_Values to database
           Input:  refference_id , calculation_id, value, IncreaseQty
           output: calculation id
        */
        public async Task<string> CreateProductCalculation(string refference_id, int calculation_id, double value, bool IncreaseQty)
        {
            var returnResult = "";
            CalculationValues _CalculatedValue = new CalculationValues();
            string CalculatedValueExistsQuery = "SELECT * FROM calculation_values WHERE calculation_id='" + calculation_id + "' and reffrence_id='" + refference_id + "';";
            using (IDbConnection conn = Connection)
            {
                var CalculatedValueExists = await conn.QueryAsync<CalculationValues>(CalculatedValueExistsQuery);
                if (CalculatedValueExists == null || CalculatedValueExists.Count() == 0)
                {
                    _CalculatedValue.reffrence_id = refference_id;
                    _CalculatedValue.calculation_id = calculation_id;
                    _CalculatedValue.value = value;
                    var result = await conn.InsertAsync<CalculationValues>(_CalculatedValue);
                    returnResult = result.ToString();
                }
                else
                {
                    _CalculatedValue = CalculatedValueExists.First();
                    if (IncreaseQty)
                    {
                        _CalculatedValue.value = _CalculatedValue.value + value;
                    }
                    else
                    {
                        _CalculatedValue.value = _CalculatedValue.value - value;
                    }

                    var result = await conn.UpdateAsync<CalculationValues>(_CalculatedValue);
                    returnResult = result.ToString();
                }

            }
            return returnResult;
        }

        /*
           Developer: Hamza haq
           Date: 17-9-19 
           Action: getting list of calculation_Values from database 
           Input: limit
           output: list of attribute
        */
        public async Task<List<CalculationValues>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<CalculationValues> result = new List<CalculationValues>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<CalculationValues>("Select * from " + TableName + " Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<CalculationValues>();
                }
                return result.ToList();
            }
        }
        /*
          Developer: Hamza haq
          Date: 17-9-19 
          Action: getting calculation_Values by id from database 
          Input: int id
          output: attribute
       */
        public async Task<CalculationValues> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<CalculationValues>(id);
                return result;
            }
        }
        /*
            Developer: Hamza haq
            Date: 17-9-19 
            Action: update calculation_Values to database
            Input: CalculationValuesToUpdate
            output: result
        */
        public async Task<Boolean> Update(CalculationValues CalculationValuesToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<CalculationValues>(CalculationValuesToUpdate);
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
