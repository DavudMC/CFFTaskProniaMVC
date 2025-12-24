using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplicationPronia.Entities.Common;

namespace WebApplicationPronia.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Precision(10,2)]
        [Range(0,double.MaxValue)]
        public double Price { get; set; }
        [Required]
        [MaxLength(512)]
        public string ImagePath { get; set; }
        public Category? Category { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
