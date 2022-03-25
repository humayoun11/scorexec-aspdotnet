using System.ComponentModel.DataAnnotations;

namespace ScoringAppReact.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}