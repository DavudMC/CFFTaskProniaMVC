using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApplicationPronia.Configurations
{
    public class InfoCardConfiguration : IEntityTypeConfiguration<InfoCard>
    {
        public void Configure(EntityTypeBuilder<InfoCard> builder)
        {
            builder.Property(x=>x.Title).IsRequired().HasMaxLength(50);
            builder.Property(x=>x.Description).IsRequired(false).HasMaxLength(256);
            builder.Property(x=>x.ImagePath).IsRequired();
        }
    }
}
