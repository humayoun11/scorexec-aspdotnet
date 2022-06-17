using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.Teams.Dto
{
    public class CreateOrUpdateGalleryDto
    {
        public long? PlayerId { get; set; }
        public long? MatchId { get; set; }
        public long? TeamId { get; set; }
        public long? GroundId { get; set; }
        public long? EventId { get; set; }
        public List<PictureDto> Galleries { get; set; }
        public int? TenantId { get; set; }
        public long Id { get; internal set; }
    }
}
