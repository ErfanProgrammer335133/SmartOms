using Application.ApplicationGuard;
using Application.DTOs.CustomerDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICustomerService
    {
        Task<Result<CustomerDto>> CreateCustomer(CreateCustomerDto model);
    }
}
