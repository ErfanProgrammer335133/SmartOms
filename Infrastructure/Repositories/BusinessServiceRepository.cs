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
    public class BusinessServiceRepository : GenericRepository<BusinessService>, IBusinessServiceRepository
    {
        private readonly Context _context;
        public BusinessServiceRepository(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<List<BusinessService>> GetByIdsAsync(List<Guid> ids)
        {
            return await _context.BusinessServices
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();
        }
    }
}
