using CommonHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badger_view.Models
{
    public class ClaimModel
    {
        public int po_id { get; set; }
        public int? inspect_claimer { get; set; }
        public int? publish_claimer { get; set; }
        public double? inspect_claimed_at { get; set; }
        public double? publish_claimed_at { get; set; }
        public string po_claimcol { get; set; }
        public string inspect_claimer_name { get; set; }
        public string publish_claimer_name { get; set; }
        public ClaimerType claim_type { get; set; }
    }
}
