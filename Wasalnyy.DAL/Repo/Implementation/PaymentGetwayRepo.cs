using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public class PaymentGetwayRepo : IPaymentGetwayRepo
    {
        private readonly WasalnyyDbContext _context;

        public PaymentGetwayRepo(WasalnyyDbContext context)
        {
            _context = context;
        }

        public async Task AddPaymentAsync(GatewayPaymentTransactions getwayPayment)
        {
            
            await _context.GatewayPayments.AddAsync(getwayPayment);

        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

    }
}