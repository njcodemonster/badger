using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericModals.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace badger_view.Helpers
{

    public class ProductHelper
    {
        private readonly BadgerApiHelper _BadgerApiHelper;
        private readonly IConfiguration _config;

        public ProductHelper(IConfiguration config)
        {
            _config = config;
            _BadgerApiHelper = new BadgerApiHelper(_config);
        }
        public async Task<string> createProductAttributesValuesAsync(int product_id, int attribute_id, int value_id)
        {
            ProductAttributeValues _productAttributeValues = new ProductAttributeValues();
            _productAttributeValues.product_id = product_id;
            _productAttributeValues.attribute_id = attribute_id;
            _productAttributeValues.value_id = value_id;

            String returnID = await _BadgerApiHelper.GenericPostAsyncString<String>(JsonConvert.SerializeObject(_productAttributeValues), "/product/createAttributesValues");
            return returnID;
        }

        public async Task<string> createProductAttributesAsync(int product_id, int attribute_id, string sku, string value_id)
        {
            ProductAttributes _productAttribute = new ProductAttributes();
            _productAttribute.product_id = product_id;
            _productAttribute.attribute_id = attribute_id;
            _productAttribute.sku = sku;
            _productAttribute.value_id = value_id;

            String returnID = await _BadgerApiHelper.GenericPostAsyncString<String>(JsonConvert.SerializeObject(_productAttribute), "/product/createProductAttribute");
            return returnID;
        }

        public async Task<string> createProductAttributesAsync(int product_id, int attribute_id, string value, int createdby, double created_at)

        {
            AttributeValues _attributeValues = new AttributeValues();
            _attributeValues.attribute_id = attribute_id;
            _attributeValues.Product_id = product_id;
            _attributeValues.value = value;
            _attributeValues.created_by = createdby;
            _attributeValues.created_at = created_at;

            String returnID = await _BadgerApiHelper.GenericPostAsyncString<String>(JsonConvert.SerializeObject(_attributeValues), "/attributevalues/create");
            return returnID;
        }
        public async Task<string> UpdateProductAttributesAsync(int product_id, int attribute_id, string value, int createdby, double created_at,int value_id)

        {
            AttributeValues _attributeValues = new AttributeValues();
            _attributeValues.attribute_id = attribute_id;
            _attributeValues.Product_id = product_id;
            _attributeValues.value = value;
            _attributeValues.created_by = createdby;
            _attributeValues.created_at = created_at;
            _attributeValues.value_id = value_id;

            String returnID = await _BadgerApiHelper.GenericPutAsyncString<String>(JsonConvert.SerializeObject(_attributeValues), "/attributevalues/update/"+ value_id);
            return returnID;
        }
        public async Task<string> DeleteProductAttributesAsync( int value_id)

        {
            String returnID = await _BadgerApiHelper.GenericDeleteAsyncString<String>( "/attributevalues/delete/" + value_id);
            return returnID;
        }
        public async Task<string> createAttributeAsync(string name)

        {
            Attributes _Attributes = new Attributes();
            _Attributes.attribute_display_name = name;
            _Attributes.attribute_type_id = 5;
            _Attributes.attribute = name;
            _Attributes.data_type = "varchar";

            String returnID = await _BadgerApiHelper.GenericPostAsyncString<String>(JsonConvert.SerializeObject(_Attributes), "/attributes/create");
            return returnID;
        }
        public async Task<string> UpdateFabric(List<Fabric> _fabrics, int user_id)
        {
            foreach (Fabric fab in _fabrics)
            {
                if (int.Parse(fab.attribute_id) == 0)
                    fab.attribute_id = await createAttributeAsync(fab.name);

                if (fab.toDelete)
                {
                    await DeleteProductAttributesAsync(fab.value_id);
                }
                else  if (fab.value_id==0)
                {
                    await AddAttributeValues(fab.product_id, int.Parse(fab.attribute_id), fab.percentage.ToString(), user_id);
                }
                else
                {
                    await UpdateProductAttributesAsync(fab.product_id, int.Parse(fab.attribute_id), fab.percentage.ToString(), user_id, 0, fab.value_id);
                }
               


            }
            return "Success";
        }

        public async Task<string> AddAttributeValues(int product_id, int attribute_id, string value, int user_id)
        {

            String attr_value_id = await createProductAttributesAsync(product_id, attribute_id, value, user_id, 0);

            String product_attribute_value_id = await createProductAttributesValuesAsync(product_id, attribute_id, int.Parse(attr_value_id));

            String product_attribute_id = await createProductAttributesAsync(product_id, attribute_id, "", attr_value_id);

            return "Success";

        }
        public async Task<List<AttributeValues>> GetFabric(int product_id)
        {
            List<AttributeValues> returnList = await _BadgerApiHelper.GenericGetAsync<List<AttributeValues>>("/attributevalues/GetAttributesbyProductID/" + product_id + "/5");

            return returnList;

        }
    }
}
