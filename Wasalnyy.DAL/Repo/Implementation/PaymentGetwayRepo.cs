using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> AddPaymentAsync(GatewayPayment getwayPayment)
        {
            
            await _context.GatewayPayments.AddAsync(getwayPayment);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        
    }
}