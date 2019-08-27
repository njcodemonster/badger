using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericModals.Models
{
    [Table("notes")]
    public partial class Notes
    {
        [Key]
        public int note_id { get; set; }
        public int ref_id { get; set; }
        public int note_type_id { get; set; }
        public string note { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
