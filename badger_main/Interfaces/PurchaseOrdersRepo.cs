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
    public interface IPurchaseOrdersRepository
    {
        Task<PurchaseOrders> GetById(int id);
        Task<List<PurchaseOrders>> GetAll(Int32 Limit);
        Task<String> Create(PurchaseOrders NewPurchaseOrder);
        Task<Boolean> Update(PurchaseOrders PurchaseOrdersToUpdate);
        Task UpdateSpecific(Dictionary<String, String> ValuePairs, String where);
        Task<string> Count();
        Task<object> GetPurchaseOrdersPageList(int start, int limit);
        Task<Object> GetOpenPOLineItemDetails(int PO_id, int Limit);
        Task<List<PurchaseOrderLineItems>> GetPOLineitems(Int32 product_id, Int32 PO_id);
        Task<List<RaStatus>> GetAllRaStatus();
        Task<List<ProductWashTypes>> GetAllWashTypes();
        Task<Object> GetSkuByProduct(string product_id);
        Task<Object> GetNameAndSizeByProductAndSku(string product_id, string sku);
        Task<object> SearchByPOAndInvoice(string search);
        Task<Object> GetPOList(string search);
    }
    public class PurchaseOrdersRepo : IPurchaseOrdersRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "purchase_orders";
        private string selectlimit = "30";
        public PurchaseOrdersRepo(IConfiguration config)
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
        Date: 7-5-19 
        Action: Insert purchase order data to database
        Input: new PurchaseOrders data
        output: string of PurchaseOrders id
        */
        public async Task<string> Create(PurchaseOrders NewPurchaseOrder)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.InsertAsync<PurchaseOrders>(NewPurchaseOrder);
                return result.ToString();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Total count of purchase order data from database
        Input: null
        output: string of PurchaseOrders count
        */
        public async Task<string> Count()
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync<String>("select count(po_id) from " + TableName + ";");
                return result.FirstOrDefault();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get All purchase order data from database
        Input: int limit
        output: list of PurchaseOrders
        */
        public async Task<List<PurchaseOrders>> GetAll(Int32 Limit)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<PurchaseOrders> result = new List<PurchaseOrders>();
                if (Limit > 0)
                {
                    result = await conn.QueryAsync<PurchaseOrders>("Select * from " + TableName + " Limit " + Limit.ToString() + ";");
                }
                else
                {
                    result = await conn.GetAllAsync<PurchaseOrders>();
                }
                return result.ToList();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order data by id from database
        Input: int id
        output: list of PurchaseOrders
        */
        public async Task<PurchaseOrders> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {

                var result = await conn.GetAsync<PurchaseOrders>(id);
                return result;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update purchase order data by id from database
        Input: purchase order data
        output: Boolean
        */
        public async Task<Boolean> Update(PurchaseOrders PurchaseOrdersToUpdate)
        {

            using (IDbConnection conn = Connection)
            {
                var result = await conn.UpdateAsync<PurchaseOrders>(PurchaseOrdersToUpdate);
                return result;
            }

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Update Specific purchase order data by id from database
        Input: fields value and where to update
        output: Boolean
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

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order line item data by purhcase order id and limit from database
        Input: int purhcase order id,int limit
        output: Dynamic object of purchase order line item
        */
        public async Task<Object> GetOpenPOLineItemDetails(int PO_id , int Limit)
        {

            dynamic OpenPoLineItemDetails = new ExpandoObject();
            string sQuery = "";
            if (Limit > 0)
            {
                sQuery = "SELECT A.*  FROM( SELECT product.product_id,product.wash_type_id,product.vendor_color_name,product.product_name,product.product_vendor_image,purchase_order_line_items.line_item_id,purchase_order_line_items.sku,attributes.attribute_display_name AS \"Size\" , purchase_order_line_items.line_item_ordered_quantity AS \"Quantity\" ,sku.weight,product_attributes.product_attribute_id FROM productdb.purchase_order_line_items , product ,product_attributes,attributes,sku where (purchase_order_line_items.product_id = product.product_id AND purchase_order_line_items.po_id = " + PO_id.ToString() + " and product_attributes.sku = purchase_order_line_items.sku AND attributes.attribute_id = product_attributes.attribute_id  and sku.sku = purchase_order_line_items.sku)) AS A  limit " + Limit + ";";
            }
            else
            {
                sQuery = "SELECT A.*  FROM( SELECT product.product_id,product.wash_type_id,product.vendor_color_name,product.product_name,product.product_vendor_image,purchase_order_line_items.line_item_id,purchase_order_line_items.sku,attributes.attribute_display_name AS \"Size\" , purchase_order_line_items.line_item_ordered_quantity AS \"Quantity\" ,sku.weight,product_attributes.product_attribute_id FROM productdb.purchase_order_line_items , product ,product_attributes,attributes,sku where (purchase_order_line_items.product_id = product.product_id AND purchase_order_line_items.po_id = " + PO_id.ToString() + " and product_attributes.sku = purchase_order_line_items.sku AND attributes.attribute_id = product_attributes.attribute_id  and sku.sku = purchase_order_line_items.sku)) AS A ";
            }

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> purchaseOrdersInfo = await conn.QueryAsync<object>(sQuery);
                OpenPoLineItemDetails.LineItemDetails = purchaseOrdersInfo;
            }
            return OpenPoLineItemDetails;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order page list data and limit from database
        Input: int limit
        output: Dynamic object of purchase order
        */
        public async Task<object> GetPurchaseOrdersPageList(int start,int limit)
        {

            dynamic poPageList = new ExpandoObject();
            string sQuery = "";
            if (limit > 0)
            {
                sQuery = "SELECT a.po_id, a.vendor_po_number, a.vendor_invoice_number, a.vendor_order_number, a.vendor_id, a.total_styles, a.shipping, a.order_date,b.vendor_name as vendor, a.delivery_window_start, a.delivery_window_end, a.po_status,a.ra_flag, a.updated_at FROM purchase_orders a left JOIN(SELECT vendor.vendor_id, vendor.vendor_name FROM vendor GROUP BY vendor.vendor_id) b ON b.vendor_id = a.vendor_id where a.po_status != 2 AND a.po_status != 4 order by a.po_id asc limit "+ start + "," + limit + ";";
            }
            else
            {
                sQuery = "SELECT a.po_id, a.vendor_po_number, a.vendor_invoice_number, a.vendor_order_number, a.vendor_id, a.total_styles, a.shipping, a.order_date,b.vendor_name as vendor, a.delivery_window_start, a.delivery_window_end, a.po_status,a.ra_flag, a.updated_at FROM purchase_orders a left JOIN(SELECT vendor.vendor_id, vendor.vendor_name FROM vendor GROUP BY vendor.vendor_id) b ON b.vendor_id = a.vendor_id where a.po_status != 2 AND a.po_status != 4 order by a.po_id asc";
            }

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> purchaseOrdersInfo = await conn.QueryAsync<object>(sQuery);
                poPageList.purchaseOrdersInfo = purchaseOrdersInfo;
            }
            return poPageList;

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order line item data by product_id and PO_id from database
        Input: int product_id, int PO_id
        output: List of purchase order line item
        */
        public async Task<List<PurchaseOrderLineItems>> GetPOLineitems(Int32 product_id, Int32 PO_id)
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<PurchaseOrderLineItems> result = new List<PurchaseOrderLineItems>();
                
                    result = await conn.QueryAsync<PurchaseOrderLineItems>("Select * from purchase_order_line_items where product_id="+ product_id + " and po_id="+ PO_id + " ;");
                
               
                return result.ToList();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-16-19 
        Action: Get all Ra status from database
        Input: null
        output: List of ra status
        */
        public async Task<List<RaStatus>> GetAllRaStatus()
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<RaStatus> result = new List<RaStatus>();
                result = await conn.QueryAsync<RaStatus>("Select ra_status_id, ra_status_name from ra_status;");
                return result.ToList();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-16-19 
        Action: Get all WashTypes from database
        Input: null
        output: List of WashTypes
        */
        public async Task<List<ProductWashTypes>> GetAllWashTypes()
        {
            using (IDbConnection conn = Connection)
            {
                IEnumerable<ProductWashTypes> result = new List<ProductWashTypes>();
                result = await conn.QueryAsync<ProductWashTypes>("Select wash_type_id,wash_type from product_wash_types;");
                return result.ToList();
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-20-19 
        Action: Returns the sku object of the given product id 
        Input: string productId
        output: ExpandoObject SkuDetails
        */
        public async Task<Object> GetSkuByProduct(string product_id)
        {
            dynamic SkuDetails = new ExpandoObject();
            string sQuery = "";
            sQuery = "Select * from `sku` where `product_id` IN (" + product_id + ")";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> skuResult = await conn.QueryAsync<object>(sQuery);
                SkuDetails = skuResult;
            }
            return SkuDetails;
        }

        /*
        Developer: Sajid Khan
        Date: 7-24-19 
        Action: Get Name And Size By Product And Sku
        Input: string product_id, string sku
        output: dynamic Object of product details
        */
        public async Task<Object> GetNameAndSizeByProductAndSku(string product_id, string sku)
        {
            dynamic ProductDetails = new ExpandoObject();
            string sQuery = "";
            sQuery = "SELECT product.product_id,product.product_name, attributes.attribute_display_name AS size FROM product, product_attributes, attributes WHERE product_attributes.product_id = product.product_id  AND attributes.attribute_id = product_attributes.attribute_id AND product_attributes.sku = '" + sku+"' AND product.product_id ="+product_id+";";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> productResult = await conn.QueryAsync<object>(sQuery);
                ProductDetails = productResult;
            }
            return ProductDetails;
        }


        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order list data by search (purchase order and invoice) from database
        Input: string search
        output: Dynamic object of purchase order and invoice
        */
        public async Task<object> SearchByPOAndInvoice(string search)
        {

            dynamic poPageList = new ExpandoObject();
            string sQuery = "";
            
            sQuery = "SELECT a.po_id, a.vendor_po_number, a.vendor_invoice_number, a.vendor_order_number, a.vendor_id, a.total_styles, a.shipping, a.order_date,b.vendor_name as vendor, a.delivery_window_start, a.delivery_window_end, a.po_status, a.updated_at FROM purchase_orders a left JOIN(SELECT vendor.vendor_id, vendor.vendor_name FROM vendor GROUP BY vendor.vendor_id) b ON b.vendor_id = a.vendor_id where (a.po_status != 2 AND a.po_status != 4) AND (a.vendor_po_number= '" + search + "' OR a.vendor_invoice_number='" + search + "' OR a.vendor_order_number='" + search + "') order by a.po_id asc;";
            
            using (IDbConnection conn = Connection)
            {
                IEnumerable<object> purchaseOrdersInfo = await conn.QueryAsync<object>(sQuery);
                poPageList.purchaseOrdersInfo = purchaseOrdersInfo;
            }
            return poPageList;

        }

        /*
        Developer: Sajid Khan
        Date: 08-09-19 
        Action: Get seach po by numbers from database 
        Input: string search
        output: dynamic list of po data
        */
        public async Task<Object> GetPOList(string search)
        {
            dynamic poDetails = new ExpandoObject();

            string sQuery = "SELECT po_id AS value, vendor_po_number AS label, 'purchase_orders' AS type FROM purchase_orders WHERE(po_status != 2 AND po_status != 4) AND(vendor_po_number LIKE '" + search + "%' OR vendor_invoice_number LIKE '" + search + "%' OR vendor_order_number LIKE '" + search+"%')";

            using (IDbConnection conn = Connection)
            {
                poDetails = await conn.QueryAsync<object>(sQuery);

            }
            return poDetails;
        }



    }
}
