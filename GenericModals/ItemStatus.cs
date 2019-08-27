using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table(" item_status")]
    public partial class ItemStatus
    {
        [Key]
        public int item_status_id { get; set; }
        public string description { get; set; }
       
        public double created_at { get; set; }
        public double updated_at { get; set; }

        internal object ToArray()
        {
            throw new NotImplementedException();
        }
    }
}
