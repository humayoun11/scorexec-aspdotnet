using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.FallOfWickets.Dto
{
    public class CreateOrUpdateFallofWicketDto
    {
        public long? Id { get; set; }
        public long MatchId { get; set; }
        public long TeamId { get; set; }
        public int First { get; set; }
        public int Second { get; set; }
        public int Third { get; set; }
        public int Fourth { get; set; }
        public int Fifth { get; set; }
        public int Sixth { get; set; }
        public int Seventh { get; set; }
        public int Eight { get; set; }
        public int Ninth { get; set; }
        public int Tenth { get; set; }
        public int? TenantId { get; set; }
    }
}
