using Dmain.Exceptions;
using Dmain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.Entities
{
    public class Wallet
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public Money Balance { get; private set; }

        private readonly List<Transaction> _transactions;

        public Wallet(Guid customerId)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            Balance = new Money(0 , Enums.CurrencyEnum.Toman);
            _transactions = new List<Transaction>();
        }
        public Wallet(Guid customerId , Enums.CurrencyEnum currency)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            Balance = new Money(0 , currency);
            _transactions = new List<Transaction>();
        }

        public Transaction Deposite(Money amount)
        {
            if (amount.Amount <= 0)
                throw new MoneyValidationException("مقدار پول نمیتواند منفی یا صفر باشد .");
            if (amount.Currency != Balance.Currency)
                throw new MoneyValidationException("واحد پولی وارد شده با واحد پولی فعلی متفاوت است .");
            Money prevBalance = Balance;

            Balance = new Money(prevBalance.Amount + amount.Amount, prevBalance.Currency);
            Transaction transaction = new Transaction(Id, amount, Enums.TransactionTypeEnum.Deposite , prevBalance , Balance);
            _transactions.Add(transaction);
            return transaction;
        } 
        
        public Transaction Withdraw(Money amount)
        {
            if (amount.Amount <= 0 || Balance.Amount - amount.Amount < 0)
                throw new MoneyValidationException("مقدار وجه درخواستی بیشتر از موجودی است .");
            Money prevBalance = Balance;

            Balance = new Money(prevBalance.Amount - amount.Amount, prevBalance.Currency);
            Transaction transaction = new Transaction(Id, new Money(amount.Amount), Enums.TransactionTypeEnum.Deposite , prevBalance , Balance);
            _transactions.Add(transaction);
            return transaction;
        }

    }
}
