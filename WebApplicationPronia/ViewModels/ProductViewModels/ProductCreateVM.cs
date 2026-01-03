using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationPronia.ViewModels.ProductViewModels
{
    public class ProductCreateVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Precision(10, 2)]
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public List<int> TagIds { get; set; }
        [Range(0, 5)]
        [Precision(2, 1)]
        public decimal Rating { get; set; }

        public IFormFile MainImage { get; set; } = null!;
        
        public IFormFile HoverImage { get; set; } = null!;

        public List<IFormFile> Images { get; set; } = [];
    }
}
