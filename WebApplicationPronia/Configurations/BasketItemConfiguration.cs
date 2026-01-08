using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApplicationPronia.Configurations
{
    public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
    {
        public void Configure(EntityTypeBuilder<BasketItem> builder)
        {
            builder.Property(x => x.Count).IsRequired();
            builder.ToTable(t => t.HasCheckConstraint("CK_Product_Count", "[Count]>0"));
            builder.HasIndex(x => new { x.ProductId, x.AppUserId }).IsUnique();
            builder.HasOne(x=>x.Product).WithMany(x=>x.BasketItems).HasForeignKey(x=>x.ProductId).HasPrincipalKey(x=>x.Id).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.AppUser).WithMany(x => x.BasketItems).HasForeignKey(x => x.AppUserId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
