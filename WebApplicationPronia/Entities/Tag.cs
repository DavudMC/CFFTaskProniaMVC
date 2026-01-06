using WebApplicationPronia.Entities.Common;

namespace WebApplicationPronia.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; } 
        public ICollection<ProductTag> ProductTags { get; set; } = [];
    }
}
