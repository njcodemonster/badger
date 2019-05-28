using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badger_view.Models
{
    public class Notes
    {
            public int note_id { get; set; }
            public int ref_id { get; set; }
            public int note_type_id { get; set; }
            public string note { get; set; }
            public int created_by { get; set; }
            public int updated_by { get; set; }
            public Double created_at { get; set; }
            public Double updated_at { get; set; }
    }
}
