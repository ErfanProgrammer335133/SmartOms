using Application.ApplicationGuard;
using Application.DTOs.BusinessServviceDTOs;
using Application.DTOs.CartDTOs;
using Application.DTOs.OrderDTOs;
using Application.Exceptions;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.IRepositories;
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
    public class CartServiceTests
    {
        private readonly Mock<ICartRepository> _mockCartRepo;
        private readonly Mock<IWalletRepository> _mockWalletRepo;
        private readonly Mock<IInvoiceRepository> _mockInvoiceRepo;
        private readonly Mock<IBusinessServiceRepository> _mockBusinessServiceRepo;
        private readonly Mock<ICustomerRepository> _mockCustomerRepo;
        private readonly Mock<IOrderRepository> _mockOrderRepo;
        private readonly Mock<IPaymentTransactionRepository> _mockTransactionRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IBusinessserviceService> _mockBusinessserviceService;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            _mockCartRepo = new Mock<ICartRepository>();
            _mockWalletRepo = new Mock<IWalletRepository>();
            _mockInvoiceRepo = new Mock<IInvoiceRepository>();
            _mockBusinessServiceRepo = new Mock<IBusinessServiceRepository>();
            _mockCustomerRepo = new Mock<ICustomerRepository>();
            _mockOrderRepo = new Mock<IOrderRepository>();
            _mockTransactionRepo = new Mock<IPaymentTransactionRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockBusinessserviceService = new Mock<IBusinessserviceService>();

            _cartService = new CartService(
                  _mockCartRepo.Object
                , _mockCustomerRepo.Object
                , _mockUnitOfWork.Object
                , _mockOrderRepo.Object
                , _mockTransactionRepo.Object
                , _mockBusinessserviceService.Object
                , _mockBusinessServiceRepo.Object
                , _mockWalletRepo.Object
                , _mockInvoiceRepo.Object
                );
        }

        public class GetNewPricesAsyncTests : CartServiceTests
        {
            [Fact]
            public async Task Should_Return_Failure_When_Cart_Not_Found()
            {
                _mockCartRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cart)null!);

                Result<CartDto> result = await _cartService.GetNewPricesAsync(Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Null(result.Value);
                Assert.Equal("سبد خرید یافت نشد .", result.ErrorMessage);
            }
            
            [Fact]
            public async Task Should_Return_Empty_CartDto_When_Cart_Items_Count_Is0()
            {
                Cart cart = new Cart(Guid.NewGuid());

                _mockCartRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(cart);
                _mockBusinessserviceService.Setup(x => x.GetPricesAsync(It.IsAny<List<Guid>>()))
                    .ReturnsAsync(new List<ServicePriceDto>());

                Result<CartDto> result = await _cartService.GetNewPricesAsync(Guid.NewGuid());

                Assert.NotNull(result.Value);
                Assert.True(result.IsSuccess);
                Assert.Empty(result.Value.Items);
            }

            [Fact]
            public async Task Should_Return_New_Prices_When_Prices_Changed()
            {
                Cart cart = new Cart(Guid.NewGuid());
                Guid serviceId1 = Guid.NewGuid();
                Guid serviceId2 = Guid.NewGuid();
                cart.AddItem(serviceId1, "title1", new Domain.ValueObjects.Money(100), 2, 3, 0);
                cart.AddItem(serviceId2, "title2", new Domain.ValueObjects.Money(200), 3, 3, 0);

                _mockCartRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(cart);

                List<ServicePriceDto> prices = new List<ServicePriceDto>
                {
                    new ServicePriceDto{ Price = new Domain.ValueObjects.Money(150) , ServiceId = serviceId1 , IsActive = true},
                    new ServicePriceDto{ Price = new Domain.ValueObjects.Money(200) , ServiceId = serviceId2 , IsActive = true}
                };

                _mockBusinessserviceService.Setup(x => x.GetPricesAsync(It.IsAny<List<Guid>>()))
                    .ReturnsAsync(prices);

                Result<CartDto> result = await _cartService.GetNewPricesAsync(Guid.NewGuid());

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);

                CartItemDto? item1 = result.Value.Items.FirstOrDefault(x => x.ServiceId == serviceId1);
                Assert.NotNull(item1);
                Assert.Equal(150, item1.UnitPrice.Amount);
                Assert.True(item1.IsChanged);
                
                CartItemDto? item2 = result.Value.Items.FirstOrDefault(x => x.ServiceId == serviceId2);
                Assert.NotNull(item2);
                Assert.Equal(200, item2.UnitPrice.Amount);
                Assert.False(item2.IsChanged);
            }
            
            [Fact]
            public async Task Should_Return_New_Prices_When_Prices_Not_Changed()
            {
                Cart cart = new Cart(Guid.NewGuid());
                Guid serviceId = Guid.NewGuid();
                cart.AddItem(serviceId, "title1", new Domain.ValueObjects.Money(100), 2, 3, 0);

                _mockCartRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(cart);

                List<ServicePriceDto> prices = new List<ServicePriceDto>
                {
                    new ServicePriceDto{ Price = new Domain.ValueObjects.Money(100) , ServiceId = serviceId, IsActive = true}
                };

                _mockBusinessserviceService.Setup(x => x.GetPricesAsync(It.IsAny<List<Guid>>()))
                    .ReturnsAsync(prices);

                Result<CartDto> result = await _cartService.GetNewPricesAsync(Guid.NewGuid());

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);

                CartItemDto? item1 = result.Value.Items.FirstOrDefault(x => x.ServiceId == serviceId);
                Assert.NotNull(item1);
                Assert.Equal(100, item1.UnitPrice.Amount);
                Assert.False(item1.IsChanged);
            }

            [Fact]
            public async Task Should_Return_Failure_When_No_Services_Found()
            {
                Cart cart = new Cart(Guid.NewGuid());
                _mockCartRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(cart);

                _mockBusinessserviceService?.Setup(x => x.GetPricesAsync(It.IsAny<List<Guid>>()))
                    .ReturnsAsync((List<ServicePriceDto>)null!);

                Result<CartDto> result = await _cartService.GetNewPricesAsync(Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Null(result.Value);
            }

            [Fact]
            public async Task Should_Remove_CartItems_That_Their_Services_Was_Not_Founded_Or_Inactive()
            {
                Cart cart = new Cart(Guid.NewGuid());
                Guid serviceId1 = Guid.NewGuid();
                Guid serviceId2 = Guid.NewGuid();
                Guid serviceId3 = Guid.NewGuid();
                cart.AddItem(serviceId1, "title 1", new Domain.ValueObjects.Money(100), 2, 2, 0);
                cart.AddItem(serviceId2, "title 2", new Domain.ValueObjects.Money(150), 4, 10, 0);
                cart.AddItem(serviceId3, "title 3", new Domain.ValueObjects.Money(200), 2, 6, 0);

                _mockCartRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(cart);

                List<ServicePriceDto> prices = new List<ServicePriceDto>
                {
                    new ServicePriceDto{ServiceId = serviceId1 , Price = new Money(150) , IsActive = true},
                    new ServicePriceDto{ServiceId = serviceId2 , Price = new Money(150) , IsActive = false},
                };

                _mockBusinessserviceService.Setup(x => x.GetPricesAsync(It.IsAny<List<Guid>>()))
                    .ReturnsAsync(prices);

                Result<CartDto> result = await _cartService.GetNewPricesAsync(Guid.NewGuid());
                int expected_Count = 1;
                decimal expected_Money = 150;

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);
                Assert.Equal(expected_Count, result.Value.Items.Count);

                CartItem? item1_In_Cart = cart.Items.FirstOrDefault(x => x.ServiceId == serviceId1);
                CartItemDto? item1_In_CartDto = result.Value.Items.FirstOrDefault(x => x.ServiceId == serviceId1);
                Assert.NotNull(item1_In_Cart);
                Assert.NotNull(item1_In_CartDto);
                Assert.Equal(expected_Money, item1_In_Cart.UnitPrice.Amount);
                Assert.Equal(expected_Money, item1_In_CartDto.UnitPrice.Amount);

                CartItem? item2_In_Cart = cart.Items.FirstOrDefault(x => x.ServiceId == serviceId2);
                CartItemDto? item2_In_CartDto = result.Value.Items.FirstOrDefault(x => x.ServiceId == serviceId2);
                Assert.Null(item2_In_Cart);
                Assert.Null(item2_In_CartDto);
                
                CartItem? item3_In_Cart = cart.Items.FirstOrDefault(x => x.ServiceId == serviceId3);
                CartItemDto? item3_In_CartDto = result.Value.Items.FirstOrDefault(x => x.ServiceId == serviceId3);
                Assert.Null(item3_In_Cart);
                Assert.Null(item3_In_CartDto);
            }
        }

        public class AddToCartAsyncTests : CartServiceTests
        {
            [Fact]
            public async Task Should_Return_Failure_When_Item_Is_Null()
            {
                Result result = await _cartService.AddToCartAsync((CartItemDto)null!, Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Equal("ورودی نا معتبر" , result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Failure_When_Customer_Was_Not_Founded()
            {
                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync((Cart)null!);

                _mockCustomerRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Customer)null!);

                CartItemDto item = new CartItemDto(Guid.NewGuid(), "title", new Money(150), 2, false, 0);

                Result result = await _cartService.AddToCartAsync(item , Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Equal("مشتری یافت نشد" , result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Failure_When_Service_Was_Not_Founded()
            {
                Cart cart = new Cart(Guid.NewGuid());
                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(cart);
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync((BusinessService)null!);
                CartItemDto item = new CartItemDto(Guid.NewGuid(), "title", new Money(150), 2, false, 0);

                Result result = await _cartService.AddToCartAsync(item, Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Equal("سرویس یافت نشد", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Failure_When_Service_Is_Free_But_Price_Is_Not0()
            {
                Cart cart = new Cart(Guid.NewGuid());
                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(cart);
                BusinessService service = new BusinessService("title", "explanation for service", new Money(0), 2, true);
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(service
                    );
                CartItemDto item = new CartItemDto(service.Id, "title", new Money(150), 2, false, 0);

                Result result = await _cartService.AddToCartAsync(item, Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Equal("این یک سرویس رایگان است و نمی تواند شامل هزینه باشد .", result.ErrorMessage);
            }
            
            [Fact]
            public async Task Should_Return_Failure_When_Service_Price_Is_Differ_Than_Items_UnitPrice()
            {
                Cart cart = new Cart(Guid.NewGuid());
                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(cart);
                BusinessService service = new BusinessService("title", "explanation for service", new Money(100), 2, false);
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(service
                    );
                CartItemDto item = new CartItemDto(service.Id, "title", new Money(150), 2, false, 0);

                Result result = await _cartService.AddToCartAsync(item, Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Equal("قیمت سرویس با قیمت ایتم وارد شده برابر نیست .", result.ErrorMessage);
            }
            
            [Fact]
            public async Task Should_Return_Failure_When_Items_Quantity_Is_Greater_Than_Sevice_MaxQuantity()
            {
                Cart cart = new Cart(Guid.NewGuid());
                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(cart);
                BusinessService service = new BusinessService("title", "explanation for service", new Money(100), 2, false);
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(service
                    );
                CartItemDto item = new CartItemDto(service.Id, "title", new Money(100), 3, false, 0);

                Result result = await _cartService.AddToCartAsync(item, Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Equal("تعداد ایتم از حدااکثر تعداد مجاز سرویس بالا تر است .", result.ErrorMessage);
            }
            
            [Fact]
            public async Task Should_Return_Success_When_And_Add_Item_To_Cart_And_Save_It_In_Db_When_All_Conditions_Pass()
            {
                Cart cart = new Cart(Guid.NewGuid());
                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(cart);
                BusinessService service = new BusinessService("title", "explanation for service", new Money(100), 2, false);
                _mockBusinessServiceRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(service
                    );
                CartItemDto item = new CartItemDto(service.Id, "title", new Money(100), 2, false, 0);

                Result result = await _cartService.AddToCartAsync(item, Guid.NewGuid());

                Assert.True(result.IsSuccess);

                _mockCartRepo.Verify(x => x.GetByCustomerIdAsync(It.IsAny<Guid>()), Times.Once);
                _mockCustomerRepo.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.AtMost(1));
                _mockBusinessServiceRepo.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
                
            }

            
        }

        public class RemoveFromCartAsyncTests : CartServiceTests
        {
            [Fact]
            public async Task Should_Retuen_Failure_When_Cart_Was_Not_Founded()
            {
                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cart)null!);

                Result result = await _cartService.RemoveFromCartAsync(Guid.NewGuid(), Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Equal("سبد خرید پیدا نشد .", result.ErrorMessage);
            }
            
            [Fact]
            public async Task Should_Retuen_Failure_When_CartItem_Was_Not_Founded()
            {
                Cart cart = new Cart(Guid.NewGuid());
                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(cart);

                Result result = await _cartService.RemoveFromCartAsync(Guid.NewGuid(), Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Equal("چنین ایتمی یافت نشد", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Retuen_Success_And_Remove_Item_From_Cart_And_Save_It_When_All_Conditions_Are_Pass()
            {
                Cart cart = new Cart(Guid.NewGuid());
                Guid serviceId = Guid.NewGuid();
                cart.AddItem(serviceId , "title" , new Money(150) , 2 , 3 , 0);
                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(cart);

                CartItem? item = cart.Items.FirstOrDefault(x => x.ServiceId == serviceId);
                if (item is null)
                    return;
                Result result = await _cartService.RemoveFromCartAsync(Guid.NewGuid(), item.Id);

                CartItem? find = cart.Items.FirstOrDefault(x => x.ServiceId == serviceId);

                Assert.True(result.IsSuccess);
                Assert.Empty(cart.Items);
                Assert.Null(find);

                _mockCartRepo.Verify(x => x.GetByCustomerIdAsync(It.IsAny<Guid>()), Times.Once);
                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
            }
        }

        public class CheckoutAsyncTests : CartServiceTests
        {
            [Fact]
            public async Task Should_Return_Failure_When_Cart_Was_Not_Founded()
            {
                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cart)null!);

                Result<OrderDto> result = await _cartService.CheckoutAsync(Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Equal("سبد خرید یافت نشد ." , result.ErrorMessage);
            }
            
            [Fact]
            public async Task Should_Return_Failure_When_Wallet_Was_Not_Founded()
            {
                Cart cart = new Cart(Guid.NewGuid());
                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(cart);

                _mockWalletRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync((Wallet)null!);

                Result<OrderDto> result = await _cartService.CheckoutAsync(Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Equal("کیف پول پیدا نشد .", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Success_When_DatabaseConcurrencyException_Occure()
            {
                Cart cart = new Cart(Guid.NewGuid());
                cart.AddItem(Guid.NewGuid(), "title", new Money(150 , Domain.Enums.CurrencyEnum.Toman) , 1, 3, 0);
                Wallet wallet = new Wallet(cart.CustomerId);
                wallet.Deposite(new Money(200 , Domain.Enums.CurrencyEnum.Toman));

                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(cart);
                _mockWalletRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(wallet);

                int countOfCall = 0;
                _mockUnitOfWork.Setup(x => x.SaveAsync())
                    .Callback(() =>
                    {
                        countOfCall++;
                        if (countOfCall == 1)
                            throw new DatabaseConcurrencyException();
                    }).Returns(Task.CompletedTask);

                Result<OrderDto> result = await _cartService.CheckoutAsync(Guid.NewGuid());
                Console.WriteLine(result.ErrorMessage);

                Assert.True(result.IsSuccess);

                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Exactly(2));
            }
            
            [Fact]
            public async Task Should_Return_Failure_When_DatabaseConcurrencyException_Occure_Two_Times()
            {
                Cart cart = new Cart(Guid.NewGuid());
                cart.AddItem(Guid.NewGuid(), "title", new Money(150 , Domain.Enums.CurrencyEnum.Toman) , 1, 3, 0);
                Wallet wallet = new Wallet(cart.CustomerId);
                wallet.Deposite(new Money(200 , Domain.Enums.CurrencyEnum.Toman));

                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(cart);
                _mockWalletRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(wallet);

                _mockUnitOfWork.Setup(x => x.SaveAsync()).ThrowsAsync(new DatabaseConcurrencyException());

                Result<OrderDto> result = await _cartService.CheckoutAsync(Guid.NewGuid());

                Assert.False(result.IsSuccess);

                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Exactly(2));
            }

            [Fact]
            public async Task Should_Return_Rollback_Wallet_When_DatabaseConcurrencyException_Occure()
            {
                Cart cart = new Cart(Guid.NewGuid());
                cart.AddItem(Guid.NewGuid(), "title", new Money(150, Domain.Enums.CurrencyEnum.Toman), 1, 3, 0);
                Wallet wallet = new Wallet(cart.CustomerId);
                wallet.Deposite(new Money(200, Domain.Enums.CurrencyEnum.Toman));

                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(cart);
                _mockWalletRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(wallet);

                _mockUnitOfWork.Setup(x => x.SaveAsync()).ThrowsAsync(new DatabaseConcurrencyException());

                Money Prev_Balence = wallet.Balance;

                Result<OrderDto> result = await _cartService.CheckoutAsync(Guid.NewGuid());

                Assert.Equal(wallet.Balance.Amount, Prev_Balence.Amount);
                Assert.Equal(wallet.Balance.Currency, Prev_Balence.Currency);
            }

            [Fact]
            public async Task Should_Return_Success_And_Create_Order_And_Create_Transaction_And_Create_Invoice_When_All_Conditions_Are_Pass()
            {
                Cart cart = new Cart(Guid.NewGuid());
                cart.AddItem(Guid.NewGuid(), "title 1", new Money(150, Domain.Enums.CurrencyEnum.Toman), 1, 3, 0);
                cart.AddItem(Guid.NewGuid(), "title 2", new Money(120, Domain.Enums.CurrencyEnum.Toman), 3, 3, 0);
                Wallet wallet = new Wallet(cart.CustomerId);
                wallet.Deposite(new Money(3 * 120 + 151 , Domain.Enums.CurrencyEnum.Toman));
                
                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(cart);
                _mockWalletRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(wallet);

                Money totalPrice = cart.TotalPrice;
                Money finalPrice = cart.FinalPrice;
                IReadOnlyCollection<CartItem> Items = cart.Items;

                Result<OrderDto> result = await _cartService.CheckoutAsync(cart.CustomerId);

                Domain.Enums.OrderStatusEnum expected_type = Domain.Enums.OrderStatusEnum.Paid;

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);
                Assert.Equal(result.Value.CustomerId, cart.CustomerId);
                Assert.Equal(result.Value.CreatedAt.Year, DateTime.UtcNow.Year);
                Assert.Equal(result.Value.CreatedAt.Month, DateTime.UtcNow.Month);
                Assert.Equal(result.Value.CreatedAt.Day, DateTime.UtcNow.Day);
                Assert.Equal(result.Value.Status, expected_type);
                Assert.NotNull(result.Value.Invoice);
                Assert.Equal(result.Value.Invoice.OrderId , result.Value.Id);
                Assert.Equal(result.Value.Invoice.TotalPrice.Amount , totalPrice.Amount);
                Assert.Equal(result.Value.Invoice.TotalPrice.Currency , totalPrice.Currency);
                Assert.Equal(result.Value.Invoice.FinalPrice.Currency , finalPrice.Currency);
                Assert.Equal(result.Value.Invoice.FinalPrice.Currency , finalPrice.Currency);

                foreach(CartItem item in Items)
                {
                    OrderItemDto? orderItem = result.Value.Items.FirstOrDefault(x => x.ServiceId == item.ServiceId);

                    Assert.NotNull(orderItem);
                    Assert.Equal(item.Title, orderItem.Title);
                    Assert.Equal(item.CreatedAt.Year, orderItem.CreatedAt.Year);
                    Assert.Equal(item.CreatedAt.Month, orderItem.CreatedAt.Month);
                    Assert.Equal(item.CreatedAt.Day, orderItem.CreatedAt.Day);
                    Assert.Equal(item.UnitPrice.Amount, orderItem.UnitPrice.Amount);
                    Assert.Equal(item.UnitPrice.Currency, orderItem.UnitPrice.Currency);
                    Assert.Equal(item.Quantity, orderItem.Quantity);

                    InvoiceItem? invoiceItem = result.Value.Invoice.Items.FirstOrDefault(x => x.ServiceId == item.ServiceId);

                    Assert.NotNull(invoiceItem);
                    Assert.Equal(item.Title, invoiceItem.ServiceTitle);
                    Assert.Equal(item.CreatedAt.Year, invoiceItem.CreatedAt.Year);
                    Assert.Equal(item.CreatedAt.Month, invoiceItem.CreatedAt.Month);
                    Assert.Equal(item.CreatedAt.Day, invoiceItem.CreatedAt.Day);
                    Assert.Equal(item.UnitPrice.Amount, invoiceItem.UnitPrice.Amount);
                    Assert.Equal(item.UnitPrice.Currency, invoiceItem.UnitPrice.Currency);
                    Assert.Equal(item.FinalPrice.Amount, invoiceItem.FinalPrice.Amount);
                    Assert.Equal(item.FinalPrice.Currency, invoiceItem.FinalPrice.Currency);
                    Assert.Equal(item.Quantity, invoiceItem.Quantity);
                    Assert.Equal(item.DiscountPercent, invoiceItem.DiscountPercent);
                }

                _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(), Times.Once);
                _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
              
            }
        }

        public class ClearCartAsyncTests : CartServiceTests
        {
            [Fact]
            public async Task Should_Return_Failure_When_Cart_Was_Not_Founded()
            {
                _mockCartRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cart)null!);

                Result result = await _cartService.ClearCartAsync(Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Equal("سبد خرید یافت نشد .", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Success_And_Clear_Cart_When_All_Conditions_Are_True()
            {
                Cart cart = new Cart(Guid.NewGuid());
                cart.AddItem(Guid.NewGuid(), "title 1", new Money(150), 2, 3, 0);
                cart.AddItem(Guid.NewGuid(), "title 2", new Money(200), 3, 3, 0);

                _mockCartRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(cart);

                Result result = await _cartService.ClearCartAsync(Guid.NewGuid());

                Assert.True(result.IsSuccess);
                Assert.Empty(cart.Items);

                _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
            }
        }

    }
}
