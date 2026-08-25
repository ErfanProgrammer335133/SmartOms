using Application.ApplicationGuard;
using Application.DTOs.CustomerDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;


        public CustomerService
            (ICustomerRepository customerRepository , IUnitOfWork unitOfWork , IUserRepository userRepository)
        {
            _customerRepo = customerRepository;
            _unitOfWork = unitOfWork;
            _userRepo = userRepository;
        }
        public async Task<Result<CustomerDto>> CreateCustomer(CreateCustomerDto model)
        {
            return await ServiceHelper.Do<CustomerDto>(async () =>
            {
                if (model is null || string.IsNullOrWhiteSpace(model.FullName))
                    return Result<CustomerDto>.Failure("ورودی نامعتبر");

                Customer? customer = await _customerRepo.GetByUserIdAsync(model.UserId);
                if (customer is not null)
                    return Result<CustomerDto>.Failure("کاربری با این شناسه کاربری از قبل وجود دارد .");

                await GetAndVerfyUser(model.UserId);

                customer = new Customer
                (
                    model.UserId,
                    model.FullName,
                    model.Email != null ? model.Email : ""
                );

                await _customerRepo.AddAsync(customer);
                await _unitOfWork.SaveAsync();

                CustomerDto dto = new CustomerDto
                {
                    Id = customer.Id , 
                    UserId = customer.UserId , 
                    Email = customer.Email != null ? customer.Email.Address : "" ,
                    FullName = customer.FullName
                };

                return Result<CustomerDto>.Success(dto);
            }, 2);

        }
            /////////////////////////////////////////////////////////////////
            /// Helper Methods
            ////////////////////////////////////////////////////////////////
        
        private async Task GetAndVerfyUser(Guid userId)
        {
            User? user = await _userRepo.GetByIdAsync(userId);
            if (user is null)
                throw new NotFoundException("هیچ کاربری با این شناسه ثبت نام نکرده است .");
            if (!user.IsActive)
                throw new DomainValidationException("حساب کاربری غیر فعال است .");
        }

    }
}

