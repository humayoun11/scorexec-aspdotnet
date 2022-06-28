using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.Grounds.Dto
{
    public class CreateOrUpdateGroundDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string ProfileUrl { get; set; }
        public PictureDto Profile { get; set; }
        public List<PictureDto> Gallery { get; set; }
    }
}
