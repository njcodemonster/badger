using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badger_view.Models
{
    public partial class CategoryOptionPage
    {
        public IEnumerable<Colors> AllColors { get; set; }
        public IEnumerable<Tags> AllTags { get; set; }
    }
    public class Colors
    {
        public Int32 value_id { get; set; }
        public string value { get; set; }
    }
    public class Tags
    {
        public Int32 attribute_id { get; set; }
        public string attribute { get; set; }
        public string attribute_display_name { get; set; }
        public string sub_heading { get; set; }

        public string isChecked { get; set; }
    }
}
