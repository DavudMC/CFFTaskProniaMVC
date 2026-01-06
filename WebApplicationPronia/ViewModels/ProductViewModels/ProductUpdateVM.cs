using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplicationPronia.Entities.Common;

namespace WebApplicationPronia.ViewModels.ProductViewModels
{
    public class ProductUpdateVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Precision(10, 2)]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public List<int> TagIds { get; set; }
        [Range(0, 5)]
        [Precision(2, 1)]
        public decimal Rating { get; set; }

        public IFormFile? MainImage { get; set; }

        public IFormFile? HoverImage { get; set; }

        public List<IFormFile>? Images { get; set; }
        public string? MainImagePath { get; set; }
        public string? HoverImagePath { get; set; }
        public List<string>? AdditionalImagePaths { get; set; } = [];
        public List<int>? AdditionalImageIds { get; set; } = [];
    }
}
