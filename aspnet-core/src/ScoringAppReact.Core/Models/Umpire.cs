using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScoringAppReact.Models
{
    public class Umpire
        : FullAuditedEntity<long>
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Contact { get; set; }
        public string Age { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }


    }
}