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

        public async Task<string> createProductAttributesAsync(int product_id, int attribute_id, string sku,string value_id)
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



    }
}
