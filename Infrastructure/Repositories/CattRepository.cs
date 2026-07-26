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
    public class CattRepository : GenericRepository<Cart>, ICartRepository
    {
        public CattRepository(Context context) : base(context)
        {
            
        }
    }
}
