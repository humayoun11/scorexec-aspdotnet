using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.Teams.Dto
{
    public class GalleryDto
    {
        public long? Id { get; set; }
        public string Url { get; set; }
        public string Blob { get; set; }
        public string Name { get; set; }
        public int? TenantId { get; set; }
        public List<GalleryDto> Pictures { get; set; }
    }
}
