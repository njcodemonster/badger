using badgerApi.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace badgerApi.Interfaces
{
    public interface ICategoryRepository
    {
        Task<String> Create(Categories newCategory);
        Task<IEnumerable<Categories>> GetSubCategoryAll();

        Task<object> GetParentCategory();
        Task<IEnumerable<Colors>> GetAllColors();
        Task<IEnumerable<Tags>> GetAllTags();
        Task<bool> CreateCategoryOption(List<CategoryOptions> newCategory);
        Task<IEnumerable<Tags>> GetAllTagsTypeWise(string id);

        Task<bool> DeleteCategoryOption(List<CategoryOptions> newCategory);

    }
    public class CategoryRepo : ICategoryRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "categories";
        private string TableProductAttributes = "product_attributes";
        private string selectlimit = "";
        public CategoryRepo(IConfiguration config)
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
        Developer: Rizwan Ali
        Date: 7-5-19 
        Action: Get all  colors from database
        Input: 
        output: list of all colors
        */
        public async Task<IEnumerable<Colors>> GetAllColors()
        {
            IEnumerable<Colors> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<Colors>("select distinct(value_id),value from attribute_values where attribute_id= 1");

            }
            return productProperties;

        }

        /*
       Developer: Rizwan Ali 
       Date: 7-5-19 
       Action: Get all  tags from database
       Input: 
       output: list of all tags
       */
        public async Task<IEnumerable<Tags>> GetAllTags()
        {
            IEnumerable<Tags> productProperties;
            using (IDbConnection conn = Connection)
            {
                productProperties = await conn.QueryAsync<Tags>("select attribute_id,attribute,attribute_display_name,sub_heading from attributes where attribute_type_id=4 order by sub_heading");

            }
            return productProperties;

        }
        /*
        Developer: Rizwan Ali 
        Date: 7-5-19 
        Action: Get all SubCategory from database
        Input: 
        output: list of all SubCategory
         */
        public async Task<IEnumerable<Categories>> GetSubCategoryAll()
        {
            IEnumerable<Categories> categories;
            using (IDbConnection conn = Connection)
            {
                categories = await conn.QueryAsync<Categories>("SELECT category_id ,category_type,category_name,category_parent_id FROM categories WHERE category_parent_id !=0");

            }
            return categories;

        }

        /*
        Developer: Rizwan Ali 
        Date: 7-5-19 
        Action: Get all  tags from database of specific cateory
        Input: 
        output: list of tags
        */
        public async Task<IEnumerable<Tags>> GetAllTagsTypeWise(string id)
        {
            int styleid = int.Parse(id);
            IEnumerable<Tags> productProperties;
            using (IDbConnection conn = Connection)
            {
                try
                {
                    productProperties = await conn.QueryAsync<Tags>
                 ("SELECT  * FROM(SELECT b.attribute_id, b.attribute, b.attribute_display_name, b.sub_heading, 'checked' AS isChecked FROM category_options a INNER JOIN attributes b ON a.attribute_id = b.attribute_id WHERE a.category_id = " + styleid + " UNION ALL SELECT b.attribute_id, b.attribute, b.attribute_display_name, b.sub_heading, '' AS isChecked FROM attributes b WHERE b.attribute_id NOT IN(SELECT b.attribute_id FROM category_options a INNER JOIN attributes b ON a.attribute_id = b.attribute_id WHERE a.category_id = " + styleid + ") AND attribute_type_id = 4) c ORDER BY c.sub_heading");

                }
                catch (Exception ex)
                {

                    productProperties = null;
                }

            }
            return productProperties;

        }

        /*
        Developer: Rizwan Ali 
        Date: 7-5-19 
        Action: Get all Parent Category from database
        Input: 
        output: list of all ParentCategory
        */
        public async Task<Object> GetParentCategory()
        {
            dynamic categories = new ExpandoObject();
            using (IDbConnection conn = Connection)
            {
                categories = await conn.QueryAsync<object>("SELECT category_id ,category_type,category_name,category_parent_id FROM categories WHERE category_parent_id =0");
            }
            return categories;

        }
        /*
        Developer: Rizwan ali
        Date: 7-7-19 
        Action: Insert new Category into database
        Input: Category data
        output: string of Category data
*/
        public async Task<string> Create(Categories categories)
        {
            using (IDbConnection conn = Connection)
            {
                long result = conn.Insert<Categories>(categories);
                return result.ToString();
            }
        }
        /*
        Developer: Rizwan ali
        Date: 7-7-19 
        Action: Insert new Category option like tags in category options into database
        Input: CategoryOptions
        output: status
*/
        public async Task<bool> CreateCategoryOption(List<CategoryOptions> category_options)
        {
            bool status = false;
            try
            {
                long result=default;
            foreach (var item in category_options)
            {
                using (IDbConnection conn = Connection)
                {
                     result = conn.Insert<CategoryOptions>(item);
                   
                }
            }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            

        }
        /*
        Developer: Rizwan ali
        Date: 7-7-19 
        Action: delete Category option like tags in category options into database
        Input: Product data to be deleted
        output: status
*/
        public async Task<bool> DeleteCategoryOption(List<CategoryOptions> category_options)
        {
            bool status = false;
            try
            {
                long result = default;
                foreach (var item in category_options)
                {
                    using (IDbConnection conn = Connection)
                    {
                        String DeleteQuery = "delete FROM category_options WHERE category_id= " + item.category_id + " and attribute_id= "+item.attribute_id;
                        var updateResult = await conn.QueryAsync<object>(DeleteQuery);
                        status = true;
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                return false;
            }


        }
    }
}
