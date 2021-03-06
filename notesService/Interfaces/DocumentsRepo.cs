﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GenericModals.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System.Data;

namespace notesService.Interfaces
{
    public interface IDocumentsRepository
    {
        Task<Documents> GetByIDAsync(int id);
        Task<List<Documents>> GetAllByReffAsync(int reff, int doc_type ,int Limit);
        Task<String> CreateAsync(Documents NewDoc);

    }
    public class DocumentsRepo : IDocumentsRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "documents";
        private string selectlimit = "30";
        public DocumentsRepo(IConfiguration config)
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
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: insert document to database
        Input: new document data
        output: string of document id
        */
        public async Task<string> CreateAsync(Documents NewDoc)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Documents>(NewDoc);
                return result.ToString();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: List of document by refference id from database
        Input: int reff,int doc_type, int Limit
        output: Dynamic List of document
        */
        public async Task<List<Documents>> GetAllByReffAsync(int reff,int doc_type, int Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Documents> result = new List<Documents>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<Documents>("Select * from " + TableName + " where ref_id="+reff.ToString()+ " and doc_type_id="+ doc_type.ToString()+"  order by doc_id DESC Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.QueryAsync<Documents>("Select * from " + TableName + " where ref_id=" + reff.ToString() + " and doc_type_id=" + doc_type.ToString() + " order by doc_id DESC Limit " + selectlimit + ";");
                }
                return result.ToList();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-13-19 
        Action: List of document by id from database
        Input: int id
        output: Dynamic List of document
        */
        public async Task<Documents> GetByIDAsync(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<Documents>(id);
                return result;
            }
        }
    }
}
