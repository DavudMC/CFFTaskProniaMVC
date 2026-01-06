using System.ComponentModel.DataAnnotations;
using WebApplicationPronia.Entities.Common;

namespace WebApplicationPronia.Entities
{
    public class ProductImage : BaseEntity
    {
        public Product? Product { get; set; }
        public int ProductId { get; set; }
        //[Required]
        //[MaxLength(512)]
        public string ImagePath { get; set; }
    }
}
