using Application.ApplicationGuard;
using Application.DTOs.TransactionDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.IRepositories;
using Domain.Repositories;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IPaymentTransactionRepository _transactionRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public WalletService(IWalletRepository walletRepository , IUnitOfWork unitOfWork,
            ICustomerRepository customerRepo , IPaymentService paymentService , IPaymentTransactionRepository paymentTransactionRepository)
        {
            _walletRepo = walletRepository;
            _unitOfWork = unitOfWork;
            _customerRepo = customerRepo;
            _paymentService = paymentService;
            _transactionRepo = paymentTransactionRepository;
        }

        public async Task<Result<PaymentTransactionDto>> DepositeAsync(DepositeDto dto)
        {
            return await ServiceHelper.Do<PaymentTransactionDto>(async () =>
            {
                if (dto is null)
                    return Result<PaymentTransactionDto>.Failure("ورودی نامعتبر است .");

                Wallet? wallet = await GetAndVerifyWallet(dto.WalletId);
                Customer? customer = await GetAndVerifyCustomer(dto.CustomerId);

                if (wallet.CustomerId != customer.Id)
                    return Result<PaymentTransactionDto>.Failure("اطلاعات وارد شده باهم مطابقت ندار .");

                PaymentTransaction transaction = await _paymentService.PayAsync(new DTOs.PaymentDTOs.PayDto
                {
                    WalletId = wallet.Id,
                    Amount = dto.Amount,
                    PreviousBalence = wallet.Balance,
                    CurrentBalence = new Money(wallet.Balance.Amount + dto.Amount.Amount),
                    Type = Domain.Enums.TransactionTypeEnum.Deposite
                });
                wallet.Deposite(dto.Amount);

                await _unitOfWork.SaveAsync();

                return Result<PaymentTransactionDto>.Success(ConvertToPaymentTransactionDto(transaction));
            } , 2);
        }

        public async Task<Result<List<PaymentTransactionDto>>> GetTransactionsHistoryAsync(Guid walletId)
        {
            return await ServiceHelper.Do<List<PaymentTransactionDto>>(async () =>
            {
                Wallet? wallet = await GetAndVerifyWallet(walletId);

                List<PaymentTransaction>? transactions = await _transactionRepo.GetTransactionsHistoryAsync(walletId);
                List<PaymentTransactionDto> dto = new List<PaymentTransactionDto>();

                foreach (PaymentTransaction payment in transactions)
                    dto.Add(ConvertToPaymentTransactionDto(payment));
                return Result<List<PaymentTransactionDto>>.Success(dto);
            }, 2);
        }

        public async Task<Result<WalletDto>> GetWalletAsync(Guid customerId)
        {
            return await ServiceHelper.Do<WalletDto>(async () =>
            {
                Wallet? wallet = await GetAndVerifyWalletByCustomerId(customerId);

                return Result<WalletDto>.Success(ConvertToWalletDto(wallet));
            }, 2);
        }

        public async Task<Result<PaymentTransactionDto>> WithdrawAsync(WithdrawDto dto)
        {
            return await ServiceHelper.Do<PaymentTransactionDto>(async () =>
            {
                if (dto is null)
                    return Result<PaymentTransactionDto>.Failure("ورودی نامعتبر است .");

                Wallet? wallet = await GetAndVerifyWallet(dto.WalletId);
                Customer? customer = await GetAndVerifyCustomer(dto.CustomerId);

                if (wallet.CustomerId != customer.Id)
                    return Result<PaymentTransactionDto>.Failure("اطلاعات وارد شده باهم مطابقت ندار .");

                PaymentTransaction transaction = await _paymentService.PayAsync(new DTOs.PaymentDTOs.PayDto
                {
                    WalletId = wallet.Id,
                    Amount = dto.Amount,
                    PreviousBalence = wallet.Balance,
                    CurrentBalence = new Money(wallet.Balance.Amount - dto.Amount.Amount),
                    Type = Domain.Enums.TransactionTypeEnum.Withdraw
                });
                wallet.Withdraw(dto.Amount);

                await _unitOfWork.SaveAsync();

                return Result<PaymentTransactionDto>.Success(ConvertToPaymentTransactionDto(transaction));
            }, 2);
        }


        //////////////////////////////////////////////////////////////////////////
        /// Helper Methods
        /////////////////////////////////////////////////////////////////////////
    
        private async Task<Wallet> GetAndVerifyWallet(Guid walletId)
        {
            return await ServiceHelper.GetEntityFromRepo
                (async () => await _walletRepo.GetByIdAsync(walletId), "کیف پول پیدا نشد .");
        }
        
        private async Task<Wallet> GetAndVerifyWalletByCustomerId(Guid customerId)
        {
            return await ServiceHelper.GetEntityFromRepo
                (async () => await _walletRepo.GetByCustomerIdAsync(customerId), "کیف پول پیدا نشد .");
        }
        
        private async Task<Customer> GetAndVerifyCustomer(Guid customerId)
        {
            return await ServiceHelper.GetEntityFromRepo
                (async () => await _customerRepo.GetByIdAsync(customerId), "کاربر پیدا نشد .");
        }

        private PaymentTransactionDto ConvertToPaymentTransactionDto(PaymentTransaction transaction)
        {
            string concurrency , transactionType;

            transactionType = transaction.TransactionType == Domain.Enums.TransactionTypeEnum.Deposite ? "Deposite" : "Withdraw";

            switch(transaction.Amount.Currency)
            {
                case Domain.Enums.CurrencyEnum.Euro: 
                    {
                        concurrency = "Euro";
                        break;
                    }
                case Domain.Enums.CurrencyEnum.Dollar: 
                    {
                        concurrency = "Dollar";
                        break;
                    }
                case Domain.Enums.CurrencyEnum.Pound: 
                    {
                        concurrency = "Pound";
                        break;
                    }
                case Domain.Enums.CurrencyEnum.Toman: 
                    {
                        concurrency = "Toman";
                        break;
                    }

                default:
                    {
                        concurrency = "Toman";
                        break;
                    }
            }
            return new PaymentTransactionDto
            {
                Id = transaction.Id,
                WalletId = transaction.WalletId,
                Amount = $"{transaction.Amount.Amount} {concurrency}",
                PreviousBalance = $"{transaction.PreviousBalance.Amount} {concurrency}",
                CurrentBalance = $"{transaction.CurrentBalance.Amount} {concurrency}",
                TransactionType = transactionType,
                CreatedAt = transaction.CreatedAt
            };
        }

        private WalletDto ConvertToWalletDto(Wallet wallet)
        {
            string currency = wallet.Balance.Currency == Domain.Enums.CurrencyEnum.Toman ? "Toman" :
                wallet.Balance.Currency == Domain.Enums.CurrencyEnum.Euro ? "Euro" :
                wallet.Balance.Currency == Domain.Enums.CurrencyEnum.Dollar ? "Dollar"
                : " Pound";

            return new WalletDto
            {
                Id = wallet.Id ,
                CustomerId = wallet.CustomerId , 
                Balence = $"{wallet.Balance.Amount} {currency}"
            };
        }
    }
}
