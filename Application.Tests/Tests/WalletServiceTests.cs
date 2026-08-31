using Application.ApplicationGuard;
using Application.DTOs.TransactionDTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepositories;
using Domain.Repositories;
using Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Application.Tests.Tests
{
    public class WalletServiceTests 
    {
        private readonly Mock<IWalletRepository> _mockWalletRepo;
        private readonly Mock<ICustomerRepository> _mockCustomerRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPaymentService> _mockPaymentService;
        private readonly Mock<IPaymentTransactionRepository> _mockPaymentRepo;
        private readonly WalletService _service;

        public WalletServiceTests()
        {
            _mockWalletRepo = new Mock<IWalletRepository>();
            _mockCustomerRepo = new Mock<ICustomerRepository>();
            _mockPaymentService = new Mock<IPaymentService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPaymentRepo = new Mock<IPaymentTransactionRepository>();

            _service = new WalletService
            (
                _mockWalletRepo.Object , 
                _mockUnitOfWork.Object ,
                _mockCustomerRepo.Object ,
                _mockPaymentService.Object,
                _mockPaymentRepo.Object
            );
        }

        public class DepositeAsyncTests : WalletServiceTests
        {
            [Fact]
            public async Task Should_Return_Failre_When_Dto_Is_Null()
            {
                Result<PaymentTransactionDto> result = await _service.DepositeAsync(null);

                Assert.False(result.IsSuccess);
                Assert.Null(result.Value);
                Assert.Equal(result.ErrorMessage, "ورودی نامعتبر است .");
            }
            
            [Fact]
            public async Task Should_Return_Failre_When_Wallet_Was_Not_Founded()
            {
                DepositeDto dto = new DepositeDto { WalletId = Guid.NewGuid(), Amount = new Money(150), CustomerId = Guid.NewGuid() };
                _mockWalletRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Wallet)null);
                
                Result<PaymentTransactionDto> result = await _service.DepositeAsync(dto);

                Assert.False(result.IsSuccess);
                Assert.Null(result.Value);
                Assert.Equal("کیف پول پیدا نشد ." , result.ErrorMessage);
            }
            
            [Fact]
            public async Task Should_Return_Failre_When_Customer_Was_Not_Founded()
            {
                Guid walletId = Guid.NewGuid();
                Guid customerId = Guid.NewGuid();
                Wallet wallet = new Wallet(customerId);
                DepositeDto dto = new DepositeDto { WalletId = walletId, Amount = new Money(150), CustomerId = customerId };

                _mockCustomerRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Customer)null);
                _mockWalletRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(wallet);
                
                Result<PaymentTransactionDto> result = await _service.DepositeAsync(dto);

                Assert.False(result.IsSuccess);
                Assert.Null(result.Value);
                Assert.Equal(result.ErrorMessage, "کاربر پیدا نشد .");
            }
            
            [Fact]
            public async Task Should_Return_Failre_When_Wallets_CustomerId_Is_Not_Equal_To_Dtos_CustomerId()
            {
                Wallet wallet = new Wallet(Guid.NewGuid());
                Customer customer = new Customer(Guid.NewGuid(), "erfan ghorbani", "eghorbani897@gmail.com");
                DepositeDto dto = new DepositeDto { WalletId = Guid.NewGuid(), Amount = new Money(150), CustomerId = Guid.NewGuid() };

                _mockCustomerRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(customer);
                _mockWalletRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(wallet);
                
                Result<PaymentTransactionDto> result = await _service.DepositeAsync(dto);

                Assert.False(result.IsSuccess);
                Assert.Null(result.Value);
                Assert.Equal(result.ErrorMessage, "اطلاعات وارد شده باهم مطابقت ندار .");
            }
            
            [Fact]
            public async Task Should_Return_Success_And_Generate_Transaction_When_All_Conditions_Pass()
            {
                Customer customer = new Customer(Guid.NewGuid(), "erfan ghorbani", "eghorbani897@gmail.com");
                Wallet wallet = new Wallet(customer.Id);
                DepositeDto dto = new DepositeDto { WalletId = wallet.Id, Amount = new Money(150), CustomerId = customer.Id };
                PaymentTransaction payment = new PaymentTransaction
                    (wallet.Id, dto.Amount, Domain.Enums.TransactionTypeEnum.Deposite, wallet.Balance, new Money(50));

                _mockCustomerRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(customer);
                _mockWalletRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(wallet);
                _mockWalletRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(wallet);
                _mockPaymentService.Setup(x => x.PayAsync(It.IsAny<Money>())).ReturnsAsync(payment);

                string currenccy = payment.Amount.Currency == Domain.Enums.CurrencyEnum.Euro ? "Euro" :
                    payment.Amount.Currency == Domain.Enums.CurrencyEnum.Pound ? "Pound" :
                    payment.Amount.Currency == Domain.Enums.CurrencyEnum.Dollar ? "Dollar" :
                    payment.Amount.Currency == Domain.Enums.CurrencyEnum.Toman ? "Toman" : "";


                Result<PaymentTransactionDto> result = await _service.DepositeAsync(dto);

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);

                Assert.Equal(result.Value.WalletId, payment.WalletId);
                Assert.Equal(result.Value.Amount, $"{payment.Amount.Amount} {currenccy}");
                Assert.Equal(result.Value.CurrentBalance, $"{payment.CurrentBalance.Amount} {currenccy}");
                Assert.Equal(result.Value.PreviousBalance, $"{payment.PreviousBalance.Amount} {currenccy}");
                Assert.Equal(result.Value.CreatedAt, payment.CreatedAt);
                Assert.Equal(wallet.Balance.Amount, 150);
            }

            
        }

        public class GetTransactionsHistoryAsyncTests : WalletServiceTests
        {
            [Fact]
            public async Task Should_Return_Failre_When_Wallet_Was_Not_Founded()
            {
                _mockWalletRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Wallet)null);

                Result<List<PaymentTransactionDto>> result = await _service.GetTransactionsHistoryAsync(Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Null(result.Value);
                Assert.Equal(result.ErrorMessage, "کیف پول پیدا نشد .");
            }

            [Fact]
            public async Task Should_Return_Success_And_Transactions_List_When_All_Conditions_Pass()
            {
                Wallet wallet = new Wallet(Guid.NewGuid());
                List<PaymentTransaction> list = new List<PaymentTransaction>
                {
                    new PaymentTransaction(wallet.Id, new Money(150) , TransactionTypeEnum.Deposite , new Money(100) , new Money(250)),
                    new PaymentTransaction(wallet.Id, new Money(50) , TransactionTypeEnum.Withdraw , new Money(120) , new Money(170)),
                    new PaymentTransaction(wallet.Id, new Money(70) , TransactionTypeEnum.Deposite , new Money(73) , new Money(143))
                };

                _mockWalletRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(wallet);
                _mockPaymentRepo.Setup(x => x.GetTransactionsHistoryAsync(It.IsAny<Guid>())).ReturnsAsync(list);

                Result<List<PaymentTransactionDto>> result = await _service.GetTransactionsHistoryAsync(Guid.NewGuid());

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);

                PaymentTransactionDto? find;

                foreach(PaymentTransaction payment in list)
                {
                    find = result.Value.FirstOrDefault(x => x.Id == payment.Id);

                    Assert.NotNull(find);
                    Assert.Equal(find.WalletId , payment.WalletId);

                    decimal amount = decimal.Parse(find.Amount.Split(' ')[0]);
                    Assert.Equal(amount, payment.Amount.Amount);
                    
                    decimal currentBalence = decimal.Parse(find.CurrentBalance.Split(' ')[0]);
                    Assert.Equal(currentBalence, payment.CurrentBalance.Amount);
                    
                    decimal previousBalence = decimal.Parse(find.PreviousBalance.Split(' ')[0]);
                    Assert.Equal(previousBalence, payment.PreviousBalance.Amount);
                }
            }
        }

        public class GetWalletAsyncTests : WalletServiceTests
        {
            [Fact]
            public async Task Should_Return_When_Wallet_Was_Not_Founded()
            {
                _mockWalletRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync((Wallet)null);
                Result<WalletDto> result = await _service.GetWalletAsync(Guid.NewGuid());

                Assert.False(result.IsSuccess);
                Assert.Null(result.Value);
                Assert.Equal("کیف پول پیدا نشد .", result.ErrorMessage);
            }

            [Fact]
            public async Task Should_Return_Success_And_WalletDto_When_All_Conditions_Pass()
            {
                Wallet wallet = new Wallet(Guid.NewGuid());
                _mockWalletRepo.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(wallet);

                Result<WalletDto> result = await _service.GetWalletAsync(Guid.NewGuid());

                string currency = wallet.Balance.Currency == CurrencyEnum.Toman ? "Toman" :
                    wallet.Balance.Currency == CurrencyEnum.Pound ? "Pound" :
                    wallet.Balance.Currency == CurrencyEnum.Dollar ? "Dollar" :
                    "Euro";

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);
                Assert.Equal(result.Value.Id, wallet.Id);
                Assert.Equal(result.Value.CustomerId, wallet.CustomerId);
                Assert.Equal(result.Value.Balence , $"{wallet.Balance.Amount} {currency}");
            }
        } 
    }
}
