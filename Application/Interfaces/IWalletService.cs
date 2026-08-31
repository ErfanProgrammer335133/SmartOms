using Application.ApplicationGuard;
using Application.DTOs.TransactionDTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IWalletService
    {
        Task<Result<PaymentTransactionDto>> DepositeAsync(DepositeDto dto); 
        Task<Result<PaymentTransactionDto>> WithdrawAsync(WithdrawDto dto); 
        Task<Result<WalletDto>> GetWalletAsync(Guid customerId); 
        Task<Result<List<PaymentTransactionDto>>> GetTransactionsHistoryAsync(Guid walletId); 

    }
}
