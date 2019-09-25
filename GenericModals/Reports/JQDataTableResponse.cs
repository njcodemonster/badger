using System;
using System.Collections.Generic;
using System.Text;

namespace GenericModals.Reports
{
    public class JQDataTableResponse
    {
        public string status { get; set; }
        public int draw { get; set; }
        public long recordsTotal { get; set; }
        public long recordsFiltered { get; set; }
        public object data { get; set; }
    }
}
