using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IBusinessServiceRepository : IGenericRepository<BusinessService>
    {
        Task<List<BusinessService>> GetByIdsAsync(List<Guid> Ids);
    }
}
