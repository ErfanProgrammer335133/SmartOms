using Application.ApplicationGuard;
using Application.DTOs.BusinessServviceDTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Tests
{
    public class BusinessserviceServiceTests
    {
        private readonly Mock<IBusinessServiceRepository> _mockBusinessServiceRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly BusinessserviceService _Service;

        public BusinessserviceServiceTests()
        {
            _mockBusinessServiceRepo = new Mock<IBusinessServiceRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _Service = new BusinessserviceService(_mockBusinessServiceRepo.Object, _mockUnitOfWork.Object);
        }

        public class ActivateAsyncTests : BusinessserviceServiceTests
        {
            [Fact]
            public async Task Should_Failure_When_Service_Was_Not_Founded()
            {
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((BusinessService)null!);

                Result result = await _Service.ActivateAsync(Guid.NewGuid());
                Assert.False(result.IsSuccess);
                Assert.Equal("چنین سرویسی یافت نشد", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Success_And_Activte_Servie_When_All_Condtions_Are_Pass()
            {
                BusinessService service = new BusinessService("title", "this is a explanation", new Money(150), 1 , false);
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(service);

                service.DeActivate();
                Assert.False(service.IsActive);

                Result result = await _Service.ActivateAsync(Guid.NewGuid());

                Assert.True(result.IsSuccess);
                Assert.True(service.IsActive);
                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
            }
        }

        public class AddAsyncTests : BusinessserviceServiceTests
        {
            [Fact]
            public async Task Should_return_Failure_When_Model_Is_Null()
            {
                Result result = await _Service.AddAsync(null!);

                Assert.False(result.IsSuccess);
                Assert.Equal("ورودی نامعتبر است .", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Success_And_Create_Service_And_Save_It_When_All_Coditions_Are_Pass()
            {
                _mockBusinessServiceRepo.Setup(x => x.AddAsync(It.IsAny<BusinessService>()))
                    .Returns(Task.CompletedTask);

                CreateServiceDto service = new CreateServiceDto
                {
                    Title = " title",
                    Explanation = "This is an exlplanation",
                    Price = new Money(150),
                    MaxQuantity = 3,
                    IsFree = false
                };

                Result result = await _Service.AddAsync(service);

                Assert.True(result.IsSuccess);

                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
                _mockBusinessServiceRepo.Verify(x => x.AddAsync(It.IsAny<BusinessService>()), Times.Once);
            }
        }

        public class DeActivateAsyncTests : BusinessserviceServiceTests
        {
            [Fact]
            public async Task Should_Failure_When_Service_Was_Not_Founded()
            {
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((BusinessService)null!);

                Result result = await _Service.ActivateAsync(Guid.NewGuid());
                Assert.False(result.IsSuccess);
                Assert.Equal("چنین سرویسی یافت نشد", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Success_And_DeActivte_Servie_When_All_Condtions_Are_Pass()
            {
                BusinessService service = new BusinessService("title", "this is a explanation", new Money(150), 1, false);
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(service);

                service.Activate();
                Assert.True(service.IsActive);

                Result result = await _Service.DeActivateAsync(Guid.NewGuid());

                Assert.True(result.IsSuccess);
                Assert.False(service.IsActive);
                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
            }
        }

        public class DecreasePrcieAsyncTests : BusinessserviceServiceTests
        {
            [Fact]
            public async Task Should_Failure_When_Service_Was_Not_Founded()
            {
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((BusinessService)null!);

                Result result = await _Service.DecreasePrcieAsync(10 , Guid.NewGuid());
                Assert.False(result.IsSuccess);
                Assert.Equal("چنین سرویسی یافت نشد", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Success_And_Decrease_Price_When_Condtions_Are_Pass()
            {
                BusinessService service = new BusinessService("title", "this is a explanation", new Money(150), 1, false);
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(service);

                Result result = await _Service.DecreasePrcieAsync(10 , Guid.NewGuid());

                decimal expected_Price = 150 - 15;

                Assert.True(result.IsSuccess);
                Assert.Equal(service.Price.Amount, expected_Price);
                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
            }
        }

        public class IncreasePrcieAsyncTests : BusinessserviceServiceTests
        {
            [Fact]
            public async Task Should_Failure_When_Service_Was_Not_Founded()
            {
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((BusinessService)null!);

                Result result = await _Service.IncreasePriceAsync(10, Guid.NewGuid());
                Assert.False(result.IsSuccess);
                Assert.Equal("چنین سرویسی یافت نشد", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Success_And_Increase_Price_When_Condtions_Are_Pass()
            {
                BusinessService service = new BusinessService("title", "this is a explanation", new Money(150), 1, false);
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(service);

                Result result = await _Service.IncreasePriceAsync(10, Guid.NewGuid());

                decimal expected_Price = 150 + 15;

                Assert.True(result.IsSuccess);
                Assert.Equal(service.Price.Amount, expected_Price);
                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
            }
        }

        public class SetFreeAsyncTests : BusinessserviceServiceTests
        {
            [Fact]
            public async Task Should_Failure_When_Service_Was_Not_Founded()
            {
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((BusinessService)null!);

                Result result = await _Service.SetFreeAsync(Guid.NewGuid());
                Assert.False(result.IsSuccess);
                Assert.Equal("چنین سرویسی یافت نشد", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Success_And_Set_Free_Service_And_Set_Price_To0_When_Condtions_Are_Pass()
            {
                BusinessService service = new BusinessService("title", "this is a explanation", new Money(150), 1, false);
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(service);

                Result result = await _Service.SetFreeAsync(Guid.NewGuid());

                decimal expected_Price = 0;

                Assert.True(result.IsSuccess);
                Assert.True(service.IsFree);
                Assert.Equal(service.Price.Amount, expected_Price);
                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
            }
        }

        public class SetPaidAsyncTests : BusinessserviceServiceTests
        {
            [Fact]
            public async Task Should_Failure_When_Service_Was_Not_Founded()
            {
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((BusinessService)null!);

                Result result = await _Service.SetPaidAsync(Guid.NewGuid() , new Money(150));
                Assert.False(result.IsSuccess);
                Assert.Equal("چنین سرویسی یافت نشد", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Success_And_Set_Paid_Service_And_Set_Price_To_Entry_Price_When_Condtions_Are_Pass()
            {
                BusinessService service = new BusinessService("title", "this is a explanation", new Money(150), 1, false);
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(service);

                Result result = await _Service.SetPaidAsync(Guid.NewGuid() , new Money(150));

                decimal expected_Price = 150;

                Assert.True(result.IsSuccess);
                Assert.False(service.IsFree);
                Assert.Equal(service.Price.Amount, expected_Price);
                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
            }
        }

        public class UpdateAsyncTests : BusinessserviceServiceTests
        {
            [Fact]
            public async Task Should_Failure_When_Service_Was_Not_Founded()
            {
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((BusinessService)null!);

                UpdateServiceDto service = new UpdateServiceDto
                {
                    Id = Guid.NewGuid(),
                    Title = "title",
                    Explanation = "This is an explanation",
                    Price = new Money(150)
                };

                Result result = await _Service.UpdateAsync(service);
                Assert.False(result.IsSuccess);
                Assert.Equal("چنین سرویسی یافت نشد", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Success_Update_Service_When_Condtions_Are_Pass()
            {
                BusinessService service = new BusinessService("title", "this is a explanation", new Money(150), 1, false);
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(service);

                UpdateServiceDto serviceDto = new UpdateServiceDto
                {
                    Id = Guid.NewGuid(),
                    Title = "title",
                    Explanation = "This is an explanation",
                    Price = new Money(150)
                };

                Result result = await _Service.UpdateAsync(serviceDto);

                Assert.True(result.IsSuccess);
                Assert.Equal(service.Price.Amount, serviceDto.Price.Amount);
                Assert.Equal(service.Title, serviceDto.Title);
                Assert.Equal(service.Explanation, serviceDto.Explanation);
                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
            }
        }
    }
}
