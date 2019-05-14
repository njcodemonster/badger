using Dapper.Contrib.Extensions;
using System;

namespace itemService.Models
{
    public partial class event_type
    {

        [Key]
        public int event_type_id { get; set; }
        public string event_type_name { get; set; }
        public string event_type_description { get; set; }
        public DateTime? created_at { get; set; }

        
    }
}
