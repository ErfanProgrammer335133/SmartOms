using Dmain.Entities;
using Dmain.IRepositories;
using Infrastructure.Database_Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PaymentTransactionRepository : GenericRepository<PaymentTransaction> , IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(Context context) : base(context)
        {
            
        }
    }
}
