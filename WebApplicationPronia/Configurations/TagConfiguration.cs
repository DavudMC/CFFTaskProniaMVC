using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApplicationPronia.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Property(x=>x.Name).IsRequired().HasMaxLength(100);
            builder.HasMany(x=>x.ProductTags).WithOne(x=>x.Tag).HasForeignKey(x=>x.TagId).HasPrincipalKey(x=>x.Id).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
