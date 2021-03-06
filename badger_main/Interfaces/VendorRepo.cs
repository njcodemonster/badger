﻿using System;
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
        Task<object> GetVendorPageList(int start, int limit);
        Task<Object> GetVendorDetailsAdressRep(Int32 id);
        Task<Object> GetVendorDetailsRep(Int32 id);
        Task<Object> GetVendorDetailsAddress(Int32 id);
        Task<Object> GetVendorsNameAndID();
        Task<Object> GetVendorTypes();
        Task<Object> GetVendorLastSku(String id);
        Task<Object> GetVendorsByColumnName(string columnName, string search);
        Task<Object> CheckVendorCodeExist(string vendorcode);
        Task<Object> GetVendor(string vendor);
        Task<Object> GetStyleNumber(string stylenumber);
        Task<Object> CheckVendorNameExist(string vendorname);
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
            Input: new vendor data
            output: vendor id
         */
        public async Task<string> Create(Vendor NewVendor)
        {
            using (IDbConnection conn = Connection)
            {
                var result = conn.Insert<Vendor>(NewVendor);
                return result.ToString() ;
            }
        }
         /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: get vendor count 
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
            Input: limit
            output: vendorsinfo
         */
        public async Task<object> GetVendorPageList(int start, int limit)
        {

            dynamic vPageList = new ExpandoObject();
            string sQuery = "";
            if(limit > 0)
            {
                sQuery = "SELECT a.vendor_id,a.vendor_type,a.vendor_name,a.vendor_code,a.has_note,b.order_count,b.last_order FROM vendor a left JOIN (SELECT count(purchase_orders.po_id) as order_count, MAX(purchase_orders.po_id) as last_order, purchase_orders.vendor_id FROM purchase_orders WHERE purchase_orders.po_status!=4 GROUP BY purchase_orders.vendor_id) b ON b.vendor_id = a.vendor_id order by b.order_count DESC limit " + start + "," + limit + ";";
            }
            else
            {
                sQuery = "SELECT a.vendor_id,a.vendor_type,a.vendor_name,a.vendor_code,a.has_note,b.order_count,b.last_order FROM vendor a left JOIN (SELECT count(purchase_orders.po_id) as order_count, MAX(purchase_orders.po_id) as last_order, purchase_orders.vendor_id FROM purchase_orders WHERE purchase_orders.po_status!=4 GROUP BY purchase_orders.vendor_id) b ON b.vendor_id = a.vendor_id order by b.order_count DESC;";
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
        /*
        Developer: Azeem Hassan
        Date: 7-11-19 
        Action: get all vendor code
        Input: vendorcode
        output: list of vendor code data
        */
        public async Task<Object> CheckVendorCodeExist(string vendorcode)
        {
            dynamic vendorDetails = new ExpandoObject();
            string sQuery = "SELECT vendor_code FROM " + TableName + " WHERE vendor_code = '" + vendorcode + "'";
            using (IDbConnection conn = Connection)
            {
                vendorDetails = await conn.QueryAsync<object>(sQuery);

            }
            return vendorDetails;
        }

        /*
        Developer: Sajid Khan
        Date: 08-09-19 
        Action: get all vendor data by vendor name
        Input: string vendor
        output: dynamic list of vendor data
        */
        public async Task<Object> GetVendor(string vendor)
        {
            dynamic vendorDetails = new ExpandoObject();
            string sQuery = "SELECT vendor_id as value, vendor_name as label, logo as image,'vendor' as type FROM " + TableName + " WHERE vendor_name LIKE '%" + vendor + "%';";
            using (IDbConnection conn = Connection)
            {
                vendorDetails = await conn.QueryAsync<object>(sQuery);

            }
            return vendorDetails;
        }
        
        /*
        Developer: Sajid Khan
        Date: 08-09-19 
        Action: get all vendor data by vendor name
        Input: string vendor
        output: dynamic list of vendor data
        */
        public async Task<Object> GetStyleNumber(string stylenumber)
        {
            dynamic vendorDetails = new ExpandoObject();
            string sQuery = "SELECT vendor_products.product_id as value, vendor_products.vendor_product_code as label,product.product_vendor_image AS image,'stylenumber' as type FROM vendor_products, product WHERE vendor_products.product_id = product.product_id AND vendor_products.vendor_product_code LIKE '%" + stylenumber + "%';";
            using (IDbConnection conn = Connection)
            {
                vendorDetails = await conn.QueryAsync<object>(sQuery);

            }
            return vendorDetails;
        }

        /*
       Developer: Azeem Hassan
       Date: 9-11-19 
       Action: get all vendor name
       Input: vendorname
       output: list of vendor name data
       */
        public async Task<Object> CheckVendorNameExist(string vendorname)
        {
            dynamic vendorDetails = new ExpandoObject();
            string sQuery = "SELECT vendor_code FROM " + TableName + " WHERE vendor_name = '" + vendorname + "'";
            using (IDbConnection conn = Connection)
            {
                vendorDetails = await conn.QueryAsync<object>(sQuery);

            }
            return vendorDetails;
        }

    }
}

