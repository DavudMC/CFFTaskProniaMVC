using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        
        public Category Category { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Range(0,5)]
        [Precision(2,1)]
        public decimal Rating { get; set; }
        public string MainImagePath { get; set; }
        public string HoverImagePath { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; } = [];
        public ICollection<ProductTag> ProductTags { get; set; } = [];
    }
}
