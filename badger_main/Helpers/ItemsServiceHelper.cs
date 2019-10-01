using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using GenericModals.Models;
using GenericModals;
using Newtonsoft.Json.Linq;
using System.Text;

namespace badgerApi.Helper
{


    public interface IItemServiceHelper
    {
        Task<List<Items>> GetItemsByOrder(int PO_id);
        Task<Object> GetAllStatus();
        Task<string> ItemUpdateById(int id, string json);
        Task<string> SkuUpdateById(int id, string json);
        Task<T> GenericPostAsync<T>(T json, String _call);
        Task<string> SetProductItemStatusForPhotoshootAsync(string json, int status);
        Task<Boolean> CheckBarcodeExist(int barcode);
        Task<List<Items>> GetItemsGroupByProductId(int PO_id);
        Task<string> ItemSpecificUpdateById(int id, string json);
        Task<object> GetBarcode(int barcode);
        Task<string> GenericGetsAsync(String _call);
        Task<T> GenericGetAsync<T>(String _call);
        Task<bool> DeleteItemByProduct(string product_id,string po_id);
        Task<object> GetItemIds(int poid);
        Task<T> GetAsync<T>(string uri);
        Task<TReturn> PostAsync<TReturn>(object json, string uri);
    }
    public class ItemsServiceHelper : IItemServiceHelper
    {
        private String ItemApiUrl = "";

        private readonly IConfiguration _config;
        public ItemsServiceHelper(IConfiguration config)
        {

            _config = config;
            ItemApiUrl = _config.GetValue<string>("Services:ItemsService");

        }
        /*
        Developer: Hamza haq
        Date: 31-8-19 
        Action: data sends to item api
        Request: Get 
        Input: Any Type and URL
        output: string
        */
        public async Task<string> GenericGetsAsync(String _call)
        {
            var client = new HttpClient();
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);
            var response = await client.GetAsync(ItemApiUrl + _call, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return data;

        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var client = new HttpClient();
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);
            var response = await client.GetAsync(ItemApiUrl + uri, HttpCompletionOption.ResponseHeadersRead);
            var data = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var responseData = JsonConvert.DeserializeObject<ResponseModel>(data, settings);
            ThorwException(responseData);
            return JsonConvert.DeserializeObject<T>(responseData.Data.ToString(), settings);
        }

        private static void ThorwException(ResponseModel responseData)
        {
            if (responseData.Status != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(responseData.Message);
            }
        }

        /*
        Developer: Hamza Haq
        Date: 31-8-19 
        Action: data sends to item api
        Request: Get 
        Input: Any Type and URL
        output: json object 
        */

        public async Task<T> GenericGetAsync<T>(String _call)
        {
            var client = new HttpClient();
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);
            var response = await client.GetAsync(ItemApiUrl + _call, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(data, settings);

        }
        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get Items by purchase order id
        URL: 
        Request: Get
        Input:  int poid
        output: dynamic object of items
        */
        public async Task<List<Items>> GetItemsByOrder(int PO_id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(ItemApiUrl + "/item/list/listforPO/" + PO_id.ToString(), HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return JsonConvert.DeserializeObject<List<Items>>(data, settings);
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get Items by purchase order id
        URL: 
        Request: Get
        Input:  int poid
        output: dynamic object of items
        */
        public async Task<List<Items>> GetItemsGroupByProductId(int PO_id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(ItemApiUrl + "/item/list/getitemsgroupbyproductid/" + PO_id.ToString(), HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return JsonConvert.DeserializeObject<List<Items>>(data, settings);
        }



        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get All status of items
        URL: 
        Request: Get
        Input:  
        output: dynamic object of items status
        */
        public async Task<object> GetAllStatus()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(ItemApiUrl + "/item/status/list", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Item update by id
        URL: 
        Request: Put
        Input:  int id, string data
        output: string of items data
        */
        public async Task<string> ItemUpdateById(int id, string json)
        {
            var client = new HttpClient();
            var response = await client.PutAsJsonAsync(ItemApiUrl + "/item/update/" + id.ToString(), json);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            return data.ToString();
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: SKU Item update by id
        URL: 
        Request: Put
        Input:  int id, string data
        output: string of items data
        */
        public async Task<string> SkuUpdateById(int id, string json)
        {
            var client = new HttpClient();
            var response = await client.PutAsJsonAsync(ItemApiUrl + "/item/update/" + id.ToString(), json);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            return data.ToString();
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Create New Item data
        URL: 
        Request: Post
        Input:  type json, string data
        output: dynamic type of items data
        */
        public async Task<T> GenericPostAsync<T>(T json, String _call)
        {
            var client = new HttpClient();
            var response = await client.PostAsJsonAsync(ItemApiUrl + _call, json);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(data, settings);

        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get Product Items by small sku
        URL: 
        Request: Get
        Input:  string product_id
        output: dynamic object of items product data of small sku
        */
        public async Task<object> GetProductItemsSmallSku(string product_id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(ItemApiUrl + "/item/GetProductItemsSmallSku/" + product_id, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Update Product Items photoshoot status
        URL: 
        Request: Post
        Input:  string json , string status
        output: string of items product data of small sku
        */
        public async Task<string> SetProductItemStatusForPhotoshootAsync(string json, int status)
        {
            var client = new HttpClient();
            var response = await client.PostAsJsonAsync(ItemApiUrl + "/item/UpdateProductItemForPhotoshoot/" + status, json);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        /*
       Developer: Sajid Khan
       Date: 7-20-19 
       Action: Check Barcode already Exist by barcode 
       Request: Get
       Input: int barcode
       output: Boolean
       */
        public async Task<Boolean> CheckBarcodeExist(int barcode)
        {
            Boolean result = false;
            var client = new HttpClient();
            var response = await client.GetAsync(ItemApiUrl + "/item/checkbarcodeexist/" + barcode.ToString(), HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            if (data == "true")
            {
                result = true;
            }
            return result;
        }

        /*
        Developer: Sajid Khan
        Date: 7-24-19 
        Action: Barcode Item update by id
        URL: 
        Request: Put
        Input:  int id, string data
        output: string of items data
        */
        public async Task<string> ItemSpecificUpdateById(int id, string json)
        {
            var client = new HttpClient();
            var response = await client.PutAsJsonAsync(ItemApiUrl + "/item/specificUpdate/" + id.ToString(), json);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            return data.ToString();
        }

        /*
        Developer: Sajid Khan
        Date: 7-7-19 
        Action: Get Product Items by small sku
        URL: 
        Request: Get
        Input:  string product_id
        output: dynamic object of items product data of small sku
        */
        public async Task<object> GetBarcode(int barcode)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(ItemApiUrl + "/item/getbarcode/" + barcode, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        /*
        Developer: Sajid Khan
        Date: 11-9-19 
        Action: Get Items by poid
        Request: Get
        Input:  int poid
        output: dynamic object of items by poid
        */
        public async Task<object> GetItemIds(int poid)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(ItemApiUrl + "/item/getitemids/" + poid, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return JsonConvert.DeserializeObject<object>(data, settings);
        }

        /*
        Developer: Rizwan Ali
        Date: 7-7-19 
        Action: Delete item by product id
        URL: 
        Request: Get
        Input:  string product_id
        output: dynamic object of items product data of small sku
        */
        public async Task<bool> DeleteItemByProduct(string id,string po_id)
        {
            var client = new HttpClient();
            var response = await client.DeleteAsync(ItemApiUrl + "/item/deleteItemByProduct/" + id.ToString()+"/"+po_id);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            return Convert.ToBoolean(data);
        }

        public async Task<TReturn> PostAsync<TReturn>(object json, string uri)
        {
            var client = new HttpClient();
            HttpResponseMessage response;
            // client.BaseAddress = new Uri(BadgerAPIURL + _call);

            if (json is JObject)
            {
                response = await client.PostAsJsonAsync(ItemApiUrl + uri, json.ToString());
            }
            else
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(json, Formatting.Indented), Encoding.UTF8, "application/json");
                response = await client.PostAsync(ItemApiUrl + uri, content);
            }

            var data = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var responseData = JsonConvert.DeserializeObject<ResponseModel>(data, settings);
            ThorwException(responseData);
            return JsonConvert.DeserializeObject<TReturn>(responseData.Data.ToString(), settings);
        }

    }
}
