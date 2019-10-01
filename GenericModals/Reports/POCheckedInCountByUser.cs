using System;
using System.Collections.Generic;
using System.Text;

namespace GenericModals.Reports
{
    public class POCheckedInCountByUser
    {
        public string checkedin_at { get; set; }
        public string username { get; set; }
        public string remarks { get; set; }
        public int? user_id { get; set; }
        public int? po_checkedin_count { get; set; }
    }
}
