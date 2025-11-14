using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Configuration
{
    public class ZoneConfig : IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            // Ignore Coordinates from being mapped as an entity
            builder.Ignore(e => e.Coordinates);

            // Then configure it as a JSON property
            builder.Property(e => e.Coordinates)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<IEnumerable<Coordinates>>(v))
                .HasColumnType("nvarchar(max)");
        }
    }
}
