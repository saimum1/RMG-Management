using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace DotNetWbapi.Models
{
    public class DeclarationSetting
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        public string QuestionsJson { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Helper method to get questions as list
        public List<string> GetQuestions()
        {
            if (string.IsNullOrEmpty(QuestionsJson))
                return new List<string>();
            
            try
            {
                return JsonSerializer.Deserialize<List<string>>(QuestionsJson) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
        
        // Helper method to set questions from list
        public void SetQuestions(List<string> questions)
        {
            try
            {
                QuestionsJson = JsonSerializer.Serialize(questions);
            }
            catch
            {
                QuestionsJson = "[]";
            }
        }
    }
}