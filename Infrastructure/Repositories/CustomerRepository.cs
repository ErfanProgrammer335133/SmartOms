
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Database_Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(Context context) : base(context)
        {
            
        }
    }
}
