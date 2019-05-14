using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class TransactionTypes
    {
        public int TransactionTypeId { get; set; }
        public string TransactionName { get; set; }
        public string TransactionDescription { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
    }
}
