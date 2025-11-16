using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Configuration
{
    public class WasalnyyHubConnectionConfig : IEntityTypeConfiguration<WasalnyyHubConnection>
    {
        public void Configure(EntityTypeBuilder<WasalnyyHubConnection> builder)
        {
            builder.HasKey(x => new {x.UserId, x.SignalRConnectionId });
        }
    }
}
