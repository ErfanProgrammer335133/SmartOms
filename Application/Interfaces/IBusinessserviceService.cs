using Application.ApplicationGuard;
using Application.DTOs.BusinessServviceDTOs;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBusinessserviceService
    {
        Task<List<ServicePriceDto>> GetPricesAsync(List<Guid> ids);
        Task<Result> AddAsync(CreateServiceDto model);
        Task<Result> UpdateAsync(UpdateServiceDto model);
        Task<Result> ActivateAsync(Guid id);
        Task<Result> DeActivateAsync(Guid id);
        Task<Result> SetFreeAsync(Guid id);
        Task<Result> SetPaidAsync(Guid id, Money price); // non-free
        Task<Result> IncreasePriceAsync(decimal percent , Guid id);
        Task<Result> DecreasePrcieAsync(decimal percent , Guid id);

    }
}
