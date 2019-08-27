using CommonHelper;
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
        public double? inspect_claimed_at { get; set; }
        public double? publish_claimed_at { get; set; }
        public string po_claimcol { get; set; }
        [Write(false)]
        public string inspect_claimer_name { get; set; }
        [Write(false)]
        public string publish_claimer_name { get; set; }
        public ClaimerType claim_type { get; set; }
    }
}
