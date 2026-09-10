using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PaymentTransaction
    {
        public Guid Id { get; private set; }
        public Guid WalletId { get; private set; }
        public Wallet Wallet { get; private set; }
        public Money Amount { get; private set; }
        public Money PreviousBalance { get; private set; }
        public Money CurrentBalance { get; private set; }
        public TransactionTypeEnum TransactionType { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public PaymentTransaction
            (Guid walletId, Money amount, TransactionTypeEnum transactionType , Money previousBalance , Money currentBalance)
        {
            if (amount == null || previousBalance == null || currentBalance == null)
                throw new MoneyValidationException("مبلغ نا معتبر .");
            Id = Guid.NewGuid();
            WalletId = walletId;
            Amount = amount;
            TransactionType = transactionType;
            CreatedAt = DateTime.UtcNow;
            PreviousBalance = previousBalance;
            CurrentBalance = currentBalance;
        }

        private PaymentTransaction()
        {
            
        }
    }
}
