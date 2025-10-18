using System.ComponentModel.DataAnnotations;

namespace DotNetWbapi.Dtos
{
    public class DeclarationSettingDto
    {
        public Guid Id { get; set; }
        
        [Required]
        public string CompanyName { get; set; } = string.Empty;
        
        [Required]
        public List<string> Questions { get; set; } = new();
    }
    
    public class CreateDeclarationSettingDto
    {
        [Required]
        public string CompanyName { get; set; } = string.Empty;
        
        [Required]
        [MinLength(1, ErrorMessage = "At least one question is required")]
        public List<string> Questions { get; set; } = new();
    }
    
    public class UpdateDeclarationSettingDto
    {
        [Required]
        public string CompanyName { get; set; } = string.Empty;
        
        [Required]
        [MinLength(1, ErrorMessage = "At least one question is required")]
        public List<string> Questions { get; set; } = new();
    }
}