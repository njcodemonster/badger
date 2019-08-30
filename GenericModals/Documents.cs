using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericModals.Models
{
    [Table("documents")]
    public class Documents
    {
        [Key]
        public int doc_id { get; set; }
        public int ref_id { get; set; }
        public int doc_type_id { get; set; }
        public string url { get; set; }
        public string notes { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
