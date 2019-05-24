using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace notesService.Models
{
    [Table("notes")]
    public partial class Notes
    {
        [Key]
        public int note_id { get; set; }
        public int ref_id { get; set; }
        public int note_type_id { get; set; }
        public string note { get; set; }
        public Double created_by { get; set; }
        public int updated_by { get; set; }
        public Double created_at { get; set; }
        public int updated_at { get; set; }
    }
}
