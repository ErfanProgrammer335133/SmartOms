
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
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly Context _context;
        public CustomerRepository(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<Customer> GetByUserIdAsync(Guid userId)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
