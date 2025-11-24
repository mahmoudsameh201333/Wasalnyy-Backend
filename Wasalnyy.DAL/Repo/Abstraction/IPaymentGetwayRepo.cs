using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Repo.Abstraction
{
    public  interface IPaymentGetwayRepo
    {


        Task AddPaymentAsync(GatewayPaymentTransactions getwayPayment);
        Task<IDbContextTransaction> BeginTransactionAsync();
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
