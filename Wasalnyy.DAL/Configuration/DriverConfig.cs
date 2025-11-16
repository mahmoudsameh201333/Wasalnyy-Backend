using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Configuration
{
    internal class DriverConfig : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.OwnsOne(e => e.Coordinates, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.Property(c => c.Lat)
                    .HasConversion<decimal>()
                    .HasColumnName("Lat")
                    .IsRequired();

                ownedNavigationBuilder.Property(c => c.Lng)
                    .HasConversion<decimal>()
                    .HasColumnName("Lng")
                    .IsRequired();
            });
        }
    }
}
