using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.EntityAdmins.Dto
{
    public class CreateOrEditEntityAdmin
    {
        public long Id { get; set; }
        public long[] UserIds { get; set; }
        public long? MatchId { get; set; }
        public long? TeamId { get; set; }
        public long? EventId { get; set; }
        public long? GroundId { get; set; }
        public long? PlayerId { get; set; }
        public int? TenantId { get; set; }
    }
}
