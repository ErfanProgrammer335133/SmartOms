using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Database_Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class WalletRepository : GenericRepository<Wallet> , IWalletRepository
    {
        private readonly Context _context;
        public WalletRepository(Context context) : base(context)
        {
            _context = context;  
        }

        public async Task<Wallet?> GetByCustomerIdAsync(Guid customerId) =>
            await _context.Wallets
                .Include("_transactions")
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
    }
}
