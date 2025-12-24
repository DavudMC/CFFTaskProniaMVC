using System.ComponentModel.DataAnnotations;
using WebApplicationPronia.Entities.Common;

namespace WebApplicationPronia.Entities
{
    public class Category : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
