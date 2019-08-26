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
        Task<PoClaim> ClaimInspect(int poId, int userId);
        Task<PoClaim> RemoveClaimInspect(int poId, int userId);
        Task<PoClaim> GetClaimInspect(int poId);
        Task<PoClaim> RemoveClaimPublish(int poId, int userId);
        Task<PoClaim> ClaimPublish(int poId, int userId);
        Task<PoClaim> GetClaimPublish(int poId);
        Task<PoClaim> GetClaim(int poId);
        Task<Object> GetPOList(string search);
    }
    public class PurchaseOrdersRepo : IPurchaseOrdersRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "purchase_orders";
        private string selectlimit = "30";
        private CommonHelper.CommonHelper _common;
        public PurchaseOrdersRepo(IConfiguration config)
        {

            _config = config;
            selectlimit = _config.GetValue<string>("configs:Default_select_Limit");
            _common = new CommonHelper.CommonHelper();

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
        public async Task<Object> GetOpenPOLineItemDetails(int PO_id, int Limit)
        {

            dynamic OpenPoLineItemDetails = new ExpandoObject();
            string sQuery = "";
            if (Limit > 0)
            {
                sQuery = "SELECT A.*  FROM( SELECT product.product_id,product.wash_type_id,product.vendor_color_name,product.product_name,product.product_vendor_image,purchase_order_line_items.line_item_id,purchase_order_line_items.sku,attributes.attribute_display_name AS \"Size\" , purchase_order_line_items.line_item_ordered_quantity AS \"Quantity\" ,sku.weight,product_attributes.product_attribute_id FROM purchase_order_line_items , product ,product_attributes,attributes,sku where (purchase_order_line_items.product_id = product.product_id AND purchase_order_line_items.po_id = " + PO_id.ToString() + " and product_attributes.sku = purchase_order_line_items.sku AND attributes.attribute_id = product_attributes.attribute_id  and sku.sku = purchase_order_line_items.sku)) AS A  limit " + Limit + ";";
            }
            else
            {
                sQuery = "SELECT A.*  FROM( SELECT product.product_id,product.wash_type_id,product.vendor_color_name,product.product_name,product.product_vendor_image,purchase_order_line_items.line_item_id,purchase_order_line_items.sku,attributes.attribute_display_name AS \"Size\" , purchase_order_line_items.line_item_ordered_quantity AS \"Quantity\" ,sku.weight,product_attributes.product_attribute_id FROM purchase_order_line_items , product ,product_attributes,attributes,sku where (purchase_order_line_items.product_id = product.product_id AND purchase_order_line_items.po_id = " + PO_id.ToString() + " and product_attributes.sku = purchase_order_line_items.sku AND attributes.attribute_id = product_attributes.attribute_id  and sku.sku = purchase_order_line_items.sku)) AS A ";
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
        public async Task<object> GetPurchaseOrdersPageList(int start, int limit)
        {
            dynamic poPageList = new ExpandoObject();
            string sQuery = @"SELECT a.po_id, a.vendor_po_number, a.vendor_invoice_number, a.vendor_order_number,
                                a.vendor_id, a.total_styles, a.shipping, a.order_date,b.vendor_name as vendor,
                                a.delivery_window_start, a.delivery_window_end, a.po_status,a.ra_flag, a.updated_at,
                                a.po_id AS po_claim_id,poc.inspect_claimer, poc.publish_claimer, u.name as inspect_claimer_name, u1.name as publish_claimer_name
                                FROM purchase_orders a INNER JOIN vendor b ON b.vendor_id = a.vendor_id 
                                LEFT JOIN po_claim poc ON a.po_id = poc.po_id
                                LEFT JOIN users u ON poc.inspect_claimer = u.user_id
                                LEFT JOIN users u1 ON poc.publish_claimer = u1.user_id
                                where a.po_status != 2 AND a.po_status != 4 order by ra_flag DESC, FIELD(a.po_status, 3, 6, 5) asc";
            if (limit > 0)
            {
                sQuery += " limit "+ start + "," + limit + ";";
            }

            using (IDbConnection conn = Connection)
            {
                IEnumerable<PurchaseOrdersInfo> purchaseOrdersInfo = await conn.QueryAsync<PurchaseOrdersInfo, PoClaim, PurchaseOrdersInfo>(sQuery,
                    map: (a, b) =>
                    {
                        a.Claim = b;
                        return a;
                    }, splitOn: "po_claim_id");
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


                string querytoRun = " Select pol.line_item_id           " +
                                    ",pol.po_id                       " +
                                    ",pol.vendor_id                   " +
                                    ",pol.sku                         " +
                                    ",pol.product_id                  " +
                                    ",pol.line_item_cost              " +
                                    ",pol.line_item_retail            " +
                                    ",pol.line_item_type              " +
                                    ",pol.line_item_ordered_quantity  " +
                                    ",pol.line_item_accepted_quantity " +
                                    ",pol.line_item_rejected_quantity " +
                                    ",pol.created_by                  " +
                                    ",pol.updated_by                  " +
                                    ",pol.created_at                  " +
                                    ",pol.updated_at                  " +
                                    ",av.value as vendor_size                 " +
                                    ",av.attribute_id" +
                                    " from purchase_order_line_items pol , product_attributes pa ,attribute_values av " +
                                    " where pol.sku=pa.sku  and av.attribute_id=pa.attribute_id and av.product_id=pa.product_id  and pol.product_id = pa.product_id and pol.product_id=" + product_id + " and pol.po_id=" + PO_id + " ;";

                result = await conn.QueryAsync<PurchaseOrderLineItems>(querytoRun);
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
            sQuery = "SELECT product.product_id,product.product_name, attributes.attribute_display_name AS size FROM product, product_attributes, attributes WHERE product_attributes.product_id = product.product_id  AND attributes.attribute_id = product_attributes.attribute_id AND product_attributes.sku = '" + sku + "' AND product.product_id =" + product_id + ";";

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

        private async Task<string> GetUsernameByClaim(int po_id, ClaimerType claimerType)
        {
            var query = $"SELECT u.name FROM po_claim pc INNER JOIN users u ON pc.[claimer] = u.user_id WHERE po_id={po_id} LIMIT 1";
            query = query.Replace("[claimer]", claimerType == ClaimerType.InspectClaimer ? "inspect_claimer" : "publish_claimer");
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QuerySingleAsync<string>(query);
                return result;
            }
        }

        public async Task<PoClaim> GetClaimInspect(int poId)
        {
            var query = $"SELECT po_id,inspect_claimer,inspect_claimed_at,u.name as inspect_claimer_name" +
                $" FROM po_claim pc INNER JOIN users u ON pc.inspect_claimer = u.user_id WHERE po_id={poId} AND inspect_claimer IS NOT NULL LIMIT 1";
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QuerySingleOrDefaultAsync<PoClaim>(query);
                return result == null ? new PoClaim() : result;
            }
        }

        public async Task<PoClaim> GetClaimPublish(int poId)
        {
            var query = $"SELECT po_id,publish_claimer,publish_claimed_at,u.name as publish_claimer_name" +
                $" FROM po_claim pc INNER JOIN users u ON pc.inspect_claimer = u.user_id WHERE po_id={poId} AND publish_claimer IS NOT NULL LIMIT 1";
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QuerySingleOrDefaultAsync<PoClaim>(query);
                return result == null ? new PoClaim() : result;
            }
        }

        public async Task<PoClaim> GetClaim(int poId)
        {
            var query = string.Format(@" SELECT po_id,inspect_claimer, inspect_claimed_at, publish_claimer,
                        publish_claimed_at,u.name as inspect_claimer_name, u1.name as publish_claimer_name 
                        FROM po_claim pc LEFT JOIN users u ON pc.inspect_claimer = u.user_id
                        LEFT JOIN users u1 ON pc.publish_claimer = u1.user_id
                        WHERE po_id={0} LIMIT 1", poId);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QuerySingleOrDefaultAsync<PoClaim>(query);
                return result == null ? new PoClaim() : result;
            }
        }

        public async Task<PoClaim> ClaimInspect(int poId, int userId)
        {
            var poClaim = new PoClaim
            {
                inspect_claimer = userId,
                po_id = poId,
                inspect_claimed_at = _common.GetTimeStemp()
            };
            string query = string.Format(@"INSERT INTO po_claim (po_id,inspect_claimer,inspect_claimed_at) VALUES ({0},{1},{2})
                ON DUPLICATE KEY UPDATE inspect_claimer = {1}, inspect_claimed_at = {2}"
            , poClaim.po_id, poClaim.inspect_claimer, poClaim.inspect_claimed_at);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.ExecuteAsync(query);
                poClaim.inspect_claimer_name = await GetUsernameByClaim(poId, ClaimerType.InspectClaimer);
            }
            return await GetClaim(poId);
        }

        public async Task<PoClaim> ClaimPublish(int poId, int userId)
        {
            var poClaim = new PoClaim
            {
                publish_claimer = userId,
                po_id = poId,
                publish_claimed_at = _common.GetTimeStemp()
            };
            string query = string.Format(@"INSERT INTO po_claim (po_id,publish_claimer,publish_claimed_at) VALUES ({0},{1},{2})
                ON DUPLICATE KEY UPDATE publish_claimer = {1}, publish_claimed_at = {2}"
            , poClaim.po_id, poClaim.publish_claimer, poClaim.publish_claimed_at);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.ExecuteAsync(query);
                poClaim.publish_claimer_name = await GetUsernameByClaim(poId, ClaimerType.PublishClaimer);
            }
            return await GetClaim(poId);
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

        public async Task<PoClaim> RemoveClaimInspect(int poId, int userId)
        {
            var poClaim = new PoClaim
            {
                inspect_claimer = null,
                po_id = poId,
                inspect_claimed_at = _common.GetTimeStemp()
            };

            using (IDbConnection conn = Connection)
            {
                var result = await conn.ExecuteAsync($"UPDATE po_claim set inspect_claimer=NULL,inspect_claimed_at='{poClaim.inspect_claimed_at}'" +
                                                $" where po_id={poId}");
                return await GetClaim(poId);
            }
        }

        public async Task<PoClaim> RemoveClaimPublish(int poId, int userId)
        {
            var poClaim = new PoClaim
            {
                publish_claimer = null,
                po_id = poId,
                publish_claimed_at = _common.GetTimeStemp()
            };

            using (IDbConnection conn = Connection)
            {
                var result = await conn.ExecuteAsync($"UPDATE po_claim set publish_claimer=NULL,publish_claimed_at='{poClaim.publish_claimed_at}'" +
                                                $" where po_id={poId}");
                return await GetClaim(poId);
            }
        }
    }
}
