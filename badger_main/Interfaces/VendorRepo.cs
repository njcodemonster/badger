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
    public interface IVendorRepository
    {
        Task<Vendor> GetById(int id);
        Task<List<Vendor>> GetAll(Int32 Limit);
        Task<String> Create(Vendor NewVendor);
        Task<Boolean> Update(Vendor VendorToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task<string> Count();
        Task<object> GetVendorPageList(int limit);
        Task<Object> GetVendorDetailsAdressRep(Int32 id);
        Task<Object> GetVendorDetailsRep(Int32 id);
        Task<Object> GetVendorDetailsAddress(Int32 id);
        Task<Object> GetVendorsNameAndID();
    }
    public class VendorRepo : IVendorRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "vendor";
        private string selectlimit = "30";
        public VendorRepo(IConfiguration config)
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

        public async Task<Object> GetVendorsNameAndID()
        {
            dynamic vendorDetails = new ExpandoObject();
            string sQuery = "SELECT vendor_id, vendor_name FROM " + TableName + ";";
            using (IDbConnection conn = Connection)
            {
                vendorDetails = await conn.QueryAsync<object>(sQuery);

            }
            return vendorDetails;
        }

        public async Task<string> Create(Vendor NewVendor)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Vendor>(NewVendor);
                return result.ToString() ;
            }
        }
        public async Task<string> Count()
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync<String>("select count(vendor_id) from "+TableName+";");
                return result.FirstOrDefault();
            }
        }
        public async Task<List<Vendor>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Vendor> result = new List<Vendor>();
                if(Limit > 0)
                {
                    result = await conn.QueryAsync<Vendor>("Select * from "+TableName+" Limit "+ Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<Vendor>();
                }
                return result.ToList();
            }
        }

       

        public async Task<Vendor> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                
                var result = await conn.GetAsync<Vendor>(id);
                return result;
            }
        }

        public async Task<Boolean> Update( Vendor VendorToUpdate)
        {
            
            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Vendor>(VendorToUpdate);
                return result;
            }
           
        }
        public async Task UpdateSpecific(Dictionary<String , String> ValuePairs, String where)
        {
            QueryHelper qHellper = new QueryHelper();
            string UpdateQuery = qHellper.MakeUpdateQuery(ValuePairs, TableName, where);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync(UpdateQuery);
               
            }

        }

        public async Task<object> GetVendorPageList(int limit)
        {

            dynamic vPageList = new ExpandoObject();
            string sQuery = "";
            if(limit > 0)
            {
                sQuery = "SELECT a.vendor_id,a.vendor_type,a.vendor_name,a.vendor_code,b.order_count,b.last_order FROM vendor a left JOIN (SELECT count(purchase_orders.po_id) as order_count, MAX(purchase_orders.po_id) as last_order, purchase_orders.vendor_id FROM purchase_orders GROUP BY purchase_orders.vendor_id) b ON b.vendor_id = a.vendor_id order by a.vendor_id asc limit " + limit+";";
            }
            else
            {
                sQuery = "SELECT a.vendor_id,a.vendor_type,a.vendor_name,a.vendor_code,b.order_count,b.last_order FROM vendor a left JOIN (SELECT count(purchase_orders.po_id) as order_count, MAX(purchase_orders.po_id) as last_order, purchase_orders.vendor_id FROM purchase_orders GROUP BY purchase_orders.vendor_id) b ON b.vendor_id = a.vendor_id order by a.vendor_id asc;";
            }

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> vendorInfo = await conn.QueryAsync<object>(sQuery);
                vPageList.vendorInfo = vendorInfo;
            }
            return vPageList;
           
        }
        public async Task<Object> GetVendorDetailsAdressRep(Int32 id)
        {
            dynamic vendorDetails = new ExpandoObject();
            string sQuery = "select * from vendor,vendor_address,vendor_contact_person where(vendor.vendor_id=" + id.ToString() + "and vendor_address.vendor_id = vendor.vendor_id and vendor_contact_person.vendor_id = vendor.vendor_id)";
            using (IDbConnection conn = Connection)
            {
                vendorDetails = await conn.QueryAsync<object>(sQuery);
                
            }
            return vendorDetails;
        }
        public async Task<Object> GetVendorDetailsAddress(Int32 id)
        {
            dynamic vendorDetails = new ExpandoObject();
            string sQuery = "select * from vendor_address where(vendor_address.vendor_id=" + id.ToString() +")";
            using (IDbConnection conn = Connection)
            {
                vendorDetails = await conn.QueryAsync<object>(sQuery);

            }
            return vendorDetails;
        }
        public async Task<Object> GetVendorDetailsRep(Int32 id)
        {
            dynamic vendorDetails = new ExpandoObject();
            string sQuery = "select * from vendor_contact_person where(vendor_contact_person.vendor_id=" + id.ToString() + ")";
            using (IDbConnection conn = Connection)
            {
                vendorDetails = await conn.QueryAsync<object>(sQuery);

            }
            return vendorDetails;
        }
    }
}

