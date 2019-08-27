using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badger_view.Models
{
   
    public partial class Categories
    {
     
        public int category_id { get; set; }
        public int category_type { get; set; }
        public string category_name { get; set; }
        public int category_parent_id { get; set; }
    }
}
