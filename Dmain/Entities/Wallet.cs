using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Wallet
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public Money Balance { get; private set; }

        private readonly List<PaymentTransaction> _transactions;

        public Wallet(Guid customerId)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            Balance = new Money(0 , Enums.CurrencyEnum.Toman);
            _transactions = new List<PaymentTransaction>();
        }
        public Wallet(Guid customerId , Enums.CurrencyEnum currency)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            Balance = new Money(0 , currency);
            _transactions = new List<PaymentTransaction>();
        }

        public PaymentTransaction Deposite(Money amount)
        {
            if (amount.Amount <= 0)
                throw new MoneyValidationException("مقدار پول نمیتواند منفی یا صفر باشد .");
            if (amount.Currency != Balance.Currency)
                throw new MoneyValidationException("واحد پولی وارد شده با واحد پولی فعلی متفاوت است .");
            Money prevBalance = Balance;

            Balance = new Money(prevBalance.Amount + amount.Amount, prevBalance.Currency);
            PaymentTransaction transaction = new PaymentTransaction(Id, amount, Enums.TransactionTypeEnum.Deposite , prevBalance , Balance);
            _transactions.Add(transaction);
            return transaction;
        } 
        
        public PaymentTransaction Withdraw(Money amount)
        {
            if (amount.Amount <= 0 || Balance.Amount - amount.Amount < 0)
                throw new MoneyValidationException("مقدار وجه درخواستی بیشتر از موجودی است .");
            if (amount.Currency != Balance.Currency)
                throw new MoneyValidationException("واحد پولی وارد شده با واحد پولی فعلی متفاوت است .");
            Money prevBalance = Balance;            

            Balance = new Money(prevBalance.Amount - amount.Amount, prevBalance.Currency);
            PaymentTransaction transaction = new PaymentTransaction(Id, new Money(amount.Amount), Enums.TransactionTypeEnum.Withdraw , prevBalance , Balance);
            _transactions.Add(transaction);
            return transaction;
        }

        public IReadOnlyCollection<PaymentTransaction> Transactions => _transactions.AsReadOnly();
    }
}
