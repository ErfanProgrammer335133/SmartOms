using Application.ApplicationGuard;
using Application.DTOs.BusinessServviceDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BusinessserviceService : IBusinessserviceService
    {
        private readonly IBusinessServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        public BusinessserviceService
            (IBusinessServiceRepository businessServiceRepository , IUnitOfWork unitOfWork)
        {
            _serviceRepository = businessServiceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> ActivateAsync(Guid id)
        {
            return await ServiceHelper.Do(async () => 
            {
                BusinessService service = await GetService(id);
                service.Activate();
                await _unitOfWork.SaveAsync();
                return Result.Success();
            }, 1);
        }

        public async Task<Result> AddAsync(CreateServiceDto model)
        {
            return await ServiceHelper.Do(async () =>
            {
                if (model is null)
                    return Result.Faliure("ورودی نامعتبر است .");
                BusinessService service = new BusinessService
                    (model.Title, model.Explanation, model.Price, model.MaxQuantity , model.IsFree);

                await _serviceRepository.AddAsync(service);
                await _unitOfWork.SaveAsync();

                return Result.Success();
            }, 1);
        }

        public async Task<Result> DeActivateAsync(Guid id)
        {
            return await ServiceHelper.Do(async () =>
            {
                BusinessService service = await GetService(id);
                service.DeActivate();
                await _unitOfWork.SaveAsync();
                return Result.Success();
            }, 1);
        }

        public async Task<Result> DecreasePrcieAsync(decimal percent, Guid id)
        {
            return await ServiceHelper.Do(async () =>
            {
                BusinessService service = await GetService(id);
                service.DecreasePrice(percent);
                await _unitOfWork.SaveAsync();
                return Result.Success();
            }, 1);
        }

        public async Task<List<ServicePriceDto>> GetPricesAsync(List<Guid> ids)
        {
            List<BusinessService> services = await _serviceRepository.GetByIdsAsync(ids);

            return services.Select(x => new ServicePriceDto { ServiceId = x.Id, Price = x.Price , IsActive = x.IsActive}).ToList();
        }

        public async Task<Result> IncreasePriceAsync(decimal percent, Guid id)
        {
            return await ServiceHelper.Do(async () =>
            {
                BusinessService service = await GetService(id);
                service.IncreasePrice(percent);
                await _unitOfWork.SaveAsync();
                return Result.Success();
            }, 1);
        }

        public async Task<Result> SetFreeAsync(Guid id)
        {
            return await ServiceHelper.Do(async () =>
            {
                BusinessService service = await GetService(id);
                service.SetFree();
                await _unitOfWork.SaveAsync();
                return Result.Success();
            }, 1);
        }

        public async Task<Result> SetPaidAsync(Guid id , Money price)
        {
            return await ServiceHelper.Do(async () =>
            {
                BusinessService service = await GetService(id);
                service.SetNotFree(price);
                await _unitOfWork.SaveAsync();
                return Result.Success();
            }, 1);
        }

        public async Task<Result> UpdateAsync(UpdateServiceDto model)
        {
            return await ServiceHelper.Do(async () =>
            {
                BusinessService service = await GetService(model.Id);
                UpdateService(service, model);
                await _unitOfWork.SaveAsync();
                return Result.Success();
            }, 1);
        }


        /////////////////////////////////////////////////////////////////////////////
        // Helper Methods
        ////////////////////////////////////////////////////////////////////////////
        
        private async Task<BusinessService> GetService(Guid id)
        {
            return await ServiceHelper.GetEntityFromRepo
                (async () => await _serviceRepository.GetByIdAsync(id), "چنین سرویسی یافت نشد");
        }

        private void UpdateService(BusinessService service , UpdateServiceDto serviceDto)
        {
            if (serviceDto is null)
                throw new DomainValidationException("ورودی نا متعبر است.");
            service.SetTitle(serviceDto.Title);
            service.SetExplenation(serviceDto.Explanation);
            service.SetPrice(serviceDto.Price);
        }
    }
}
