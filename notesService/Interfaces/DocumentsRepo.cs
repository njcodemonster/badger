using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using notesService.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System.Data;

namespace notesService.Interfaces
{
    public interface IDocumentsRepository
    {
        Task<Documents> GetByIDAsync(int id);
        Task<List<Documents>> GetAllByReffAsync(int reff, int type ,int Limit);
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
        public async Task<string> CreateAsync(Documents NewDoc)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Documents>(NewDoc);
                return result.ToString();
            }
        }

        public async Task<List<Documents>> GetAllByReffAsync(int reff,int type,int Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Documents> result = new List<Documents>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<Documents>("Select * from " + TableName + " where ref_id="+reff.ToString()+ " and doc_type_id="+type.ToString()+"  order by doc_id DESC Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.QueryAsync<Documents>("Select * from " + TableName + " where ref_id=" + reff.ToString() + " and doc_type_id=" + type.ToString() + " order by doc_id DESC Limit " + selectlimit + ";");
                }
                return result.ToList();
            }
        }

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
