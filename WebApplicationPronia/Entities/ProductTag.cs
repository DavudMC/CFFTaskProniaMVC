using WebApplicationPronia.Entities.Common;

namespace WebApplicationPronia.Entities
{
    public class ProductTag : BaseEntity
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public Tag Tag { get; set; }
        public int TagId { get; set; }
    }
}
