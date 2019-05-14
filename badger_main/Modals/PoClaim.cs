using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class PoClaim
    {
        public int PoId { get; set; }
        public int? InspectClaimer { get; set; }
        public int? PublishClaimer { get; set; }
        public decimal? InspectClaimedAt { get; set; }
        public decimal? PublishClaimedAt { get; set; }
        public string PoClaimcol { get; set; }
    }
}
