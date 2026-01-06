using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApplicationPronia.Configurations
{
    public class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
    {
        public void Configure(EntityTypeBuilder<ProductTag> builder)
        {
            builder.HasIndex(x=> new {x.ProductId, x.TagId}).IsUnique();
            builder.HasOne(x=>x.Product).WithMany(x=>x.ProductTags).HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Tag).WithMany(x => x.ProductTags).HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
