using Dmain.Enums;
using Dmain.Exceptions;
using Dmain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid WalletId { get; private set; }
        public Money Amount { get; private set; }
        public Money PreviousBalance { get; private set; }
        public Money CurrentBalance { get; private set; }
        public TransactionTypeEnum TransactionType { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Transaction
            (Guid walletId, Money amount, TransactionTypeEnum transactionType , Money previousBalance , Money currentBalance)
        {
            if (amount == null)
                throw new MoneyValidationException("مبلغ نا معتبر .");
            Id = Guid.NewGuid();
            WalletId = walletId;
            Amount = amount;
            TransactionType = transactionType;
            CreatedAt = DateTime.UtcNow;
            PreviousBalance = previousBalance;
            CurrentBalance = currentBalance;
        }
    }
}
