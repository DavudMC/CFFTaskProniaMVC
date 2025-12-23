using System.ComponentModel.DataAnnotations;

namespace WebApplicationPronia.Entities
{
    public class InfoCard
    {
        public int Id { get; set; }
        [MaxLength(20)]
        [MinLength(3)]
        [Required]
        public string Title { get; set; } = null!;
        public string? Description { get; set; } 
        [Required]
        public string ImagePath { get; set; } = null!;
    }
}
