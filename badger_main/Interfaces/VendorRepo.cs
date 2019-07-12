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
        Task<Object> GetVendorTypes();
        Task<Object> GetVendorLastSku(String id);
        Task<Object> GetVendorsByColumnName(string columnName, string search);
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

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get All vendors name and id
        URL: 
        Request GET
        Input: null
        output: list of vendors data
         */
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

        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: getting vendor types from database
            URL: 
            Request GET
            Input: null
            output: vendor types
         */
        public async Task<Object> GetVendorTypes()
        {
            dynamic VendorTypes = new ExpandoObject();
            string sQuery = "SELECT * FROM vendor_types";
            using (IDbConnection conn = Connection)
            {
                VendorTypes = await conn.QueryAsync<object>(sQuery);

            }
            return VendorTypes;
        }
        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: insert vendor data to database
            URL: 
            Request:POST
            Input: new vendor data
            output: vendor id
         */
        public async Task<string> Create(Vendor NewVendor)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<Vendor>(NewVendor);
                return result.ToString() ;
            }
        }
         /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: get vendor count 
            URL: 
            Request:GET
            Input: null
            output: vendor count
         */
        public async Task<string> Count()
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync<String>("select count(vendor_id) from "+TableName+";");
                return result.FirstOrDefault();
            }
        }

        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: get vendors with limit 
            URL: 
            Request:GET
            Input: limit
            output: vendors list
         */
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


        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: get vendor from database by id
            URL: 
            Request:GET
            Input: vendor id
            output: vendor
         */
        public async Task<Vendor> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                
                var result = await conn.GetAsync<Vendor>(id);
                return result;
            }
        }
        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: update vendor to database by id
            URL: 
            Request:PUT
            Input: vendor data
            output: result
         */
        public async Task<Boolean> Update( Vendor VendorToUpdate)
        {
            
            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<Vendor>(VendorToUpdate);
                return result;
            }
           
        }
        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: updating specific fields to database
            URL: 
            Request:PUT
            Input: fields value and where to repair
            output: result
         */
        public async Task UpdateSpecific(Dictionary<String , String> ValuePairs, String where)
        {
            QueryHelper qHellper = new QueryHelper();
            string UpdateQuery = qHellper.MakeUpdateQuery(ValuePairs, TableName, where);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync(UpdateQuery);
               
            }

        }
        /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: getting vendor info from database
            URL: 
            Request:GET
            Input: limit
            output: vendorsinfo
         */
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
        /*
           Developer: Azeem Hassan
           Date: 7-5-19 
           Action: getting vendor details address and repo from database by vendor id
           URL: 
           Request:GET
           Input: vendor id
           output: vendorDetails
        */
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
        /*
           Developer: Azeem Hassan
           Date: 7-5-19 
           Action: getting vendor address from database by vendor id
           URL: 
           Request:GET
           Input: vendor id
           output: vendorAddressDetails
        */
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
        /*
           Developer: Azeem Hassan
           Date: 7-5-19 
           Action: getting vendor rep Details from database by vendor id
           URL: 
           Request:GET
           Input: vendor id
           output: vendorRepoDetails
        */
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
        /*
           Developer: Azeem Hassan
           Date: 7-5-19 
           Action: getting vendor last sku from database by vendor id
           URL: 
           Request:GET
           Input: vendor id
           output: vendorLastSku
        */
        public async Task<Object> GetVendorLastSku(string id)
        {
             dynamic vendorLastSku = new ExpandoObject();
            string sQuery = "select sku_family from product where vendor_id = "+ id+" order by product_id desc limit 1; ";
            using (IDbConnection conn = Connection)
            {
                vendorLastSku = await conn.QueryAsync<object>(sQuery);

            }
            return vendorLastSku;

        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get All vendors name and id
        URL: 
        Request GET
        Input: null
        output: list of vendors data
         */
        public async Task<Object> GetVendorsByColumnName(string columnName, string search)
        {
            dynamic vendorDetails = new ExpandoObject();
            string sQuery = "SELECT vendor_id as value, "+ columnName + " as label FROM " + TableName + " WHERE " + columnName + " LIKE '%" + search+"%';";
            using (IDbConnection conn = Connection)
            {
                vendorDetails = await conn.QueryAsync<object>(sQuery);

            }
            return vendorDetails;
        }

    }
}

