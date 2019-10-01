using System;
using System.Collections.Generic;
using System.Text;

namespace GenericModals.Reports
{
    public class PoCountByUser
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public int po_count { get; set; }
        public string created_at { get; set; }
        public string remarks { get; set; }
        public string styles { get; set; }
    }
}
