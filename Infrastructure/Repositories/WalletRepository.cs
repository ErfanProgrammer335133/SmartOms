using Dmain.Entities;
using Dmain.Repositories;
using Infrastructure.Database_Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class WalletRepository : GenericRepository<Wallet> , IWalletRepository
    {
        public WalletRepository(Context context) : base(context)
        {
            
        }
    }
}
