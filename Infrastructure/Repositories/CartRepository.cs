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
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly Context _context;
        public CartRepository(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByCustomerIdAsync(Guid customerId) => 
            await _context.Carts
                .Include("_items")
                .SingleOrDefaultAsync(x => x.CustomerId == customerId);

        public async Task<Cart?> GetByIdWithItemsAsync(Guid id) =>
            await _context.Carts
                .Include("_items")
                .SingleOrDefaultAsync(x => x.Id == id);

    }
}
