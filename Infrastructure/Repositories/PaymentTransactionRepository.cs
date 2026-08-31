using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Database_Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PaymentTransactionRepository : GenericRepository<PaymentTransaction> , IPaymentTransactionRepository
    {
        private readonly Context _context;
        public PaymentTransactionRepository(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<List<PaymentTransaction>> GetTransactionsHistoryAsync(Guid walletId)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(x => x.WalletId == walletId)
                .ToListAsync();
        }

    }
}
