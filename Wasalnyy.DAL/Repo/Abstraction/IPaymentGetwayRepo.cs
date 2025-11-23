using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Repo.Abstraction
{
    public  interface IPaymentGetwayRepo
    {


        Task<bool> AddPaymentAsync(GatewayPayment getwayPayment);
      
    }
}
