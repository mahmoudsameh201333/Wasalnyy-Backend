using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.DAL.Configuration
{
    public class WalletTransactionLogsConfig : IEntityTypeConfiguration<WalletTransactionLogs>
    {
        public void Configure(EntityTypeBuilder<WalletTransactionLogs> builder)
        {
            builder.ToTable("WalletTransactionLogs"); // only rename
        }
    }
}
