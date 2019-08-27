using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("po_claim")]
    public partial class PoClaim
    {
        [Key]
        public int po_id { get; set; }
        public int? inspect_claimer { get; set; }
        public int? publish_claimer { get; set; }
        public decimal? inspect_claimed_at { get; set; }
        public decimal? publish_claimed_at { get; set; }
        public string po_claimcol { get; set; }
    }
}
