using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScoringAppReact.Models
{
    public class Gallery : FullAuditedEntity<long>
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
    }
}