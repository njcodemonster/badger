using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("transaction_types")]
    public partial class TransactionTypes
    {
        public int transaction_type_id { get; set; }
        public string transaction_name { get; set; }
        public string transaction_description { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
