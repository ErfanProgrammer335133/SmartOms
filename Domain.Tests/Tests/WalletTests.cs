using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Tests
{
    public class WalletTests
    {
        
        private void Check_Wallet_Correctly_Create(Wallet wallet ,Guid customerId , CurrencyEnum currency)
        {
            int count_Of_Transactions = 0;
            decimal initial_Balence = 0;

            Assert.NotNull(wallet);
            Assert.NotNull(wallet.Transactions);
            Assert.Equal(count_Of_Transactions, wallet.Transactions.Count());
            Assert.Equal(customerId, wallet.CustomerId);
            Assert.Equal(initial_Balence, wallet.Balance.Amount);
            Assert.Equal(currency, wallet.Balance.Currency);
        }

        [Fact]
        public void Constructor_Should_Create_Wallet_Correctly_Without_Currency()
        {
            Guid customerId = Guid.NewGuid();
            Wallet wallet = new Wallet(customerId);

            Check_Wallet_Correctly_Create(wallet, customerId, CurrencyEnum.Toman);
        }
        
        [Fact]
        public void Constructor_Should_Create_Wallet_Correctly_With_Currency()
        {
            Guid customerId = Guid.NewGuid();
            Wallet wallet = new Wallet(customerId , CurrencyEnum.Euro);

            Check_Wallet_Correctly_Create(wallet, customerId, CurrencyEnum.Euro);
        }

        [Fact]
        public void Deposite_Should_Throw_MoneyValidationException_When_Amount_Is_Less_Than0()
        {
            Wallet wallet = new Wallet(Guid.NewGuid());
            Assert.Throws<MoneyValidationException>(() => wallet.Deposite(new ValueObjects.Money(-1)));
        }

        [Fact]
        public void Deposite_Should_Throw_MoneyValidationException_When_Currency_Is_Differ_Than_Balence_Currency()
        {
            Wallet wallet = new Wallet(Guid.NewGuid());
            Assert.Throws<MoneyValidationException>(() => wallet.Deposite(new ValueObjects.Money(100 , CurrencyEnum.Euro)));
        }

        [Fact]
        public void Deposite_Should_Increase_Balence_Correctly()
        {
            Wallet wallet = new Wallet(Guid.NewGuid());
            decimal prev_Amount = 0;
            wallet.Deposite(new ValueObjects.Money(100));
            decimal after_Amount = 100;
            Assert.Equal(prev_Amount + after_Amount, wallet.Balance.Amount);
        }

        [Fact]
        public void Deposite_Should_Create_PaymentTransaction_And_Add_It_To_Transactions_List()
        {
            Wallet wallet = new Wallet(Guid.NewGuid());
            decimal prev_Balence_Amount = 0;
            PaymentTransaction transaction =  wallet.Deposite(new ValueObjects.Money(100));
            decimal new_Balence_Amount = 100;

            Assert.NotNull(transaction);
            Assert.Equal(prev_Balence_Amount, transaction.PreviousBalance.Amount);
            Assert.Equal(new_Balence_Amount, transaction.CurrentBalance.Amount);
            Assert.Equal(wallet.Id, transaction.WalletId);
            Assert.Equal(TransactionTypeEnum.Deposite, transaction.TransactionType);

            PaymentTransaction? find = wallet.Transactions.FirstOrDefault(x => x.Id == transaction.Id);

            Assert.NotNull(find);
            Assert.Equal(transaction.PreviousBalance.Amount, find.PreviousBalance.Amount);
            Assert.Equal(transaction.PreviousBalance.Currency, find.PreviousBalance.Currency);
            Assert.Equal(transaction.CurrentBalance.Amount, find.CurrentBalance.Amount);
            Assert.Equal(transaction.CurrentBalance.Currency, find.CurrentBalance.Currency);
            Assert.Equal(transaction.Id, find.Id);
            Assert.Equal(transaction.WalletId, find.WalletId);
            Assert.Equal(transaction.TransactionType, find.TransactionType);
        }

        [Fact]
        public void Withdraw_Should_Throw_MoneyValidationException_When_Amount_Is_Less_Than0()
        {
            Wallet wallet = new Wallet(Guid.NewGuid());
            Assert.Throws<MoneyValidationException>(() => wallet.Withdraw(new ValueObjects.Money(-1)));
        }
        
        [Fact]
        public void Withdraw_Should_Throw_MoneyValidationException_When_Balence_Minus_Amount_Is_Less_Than0()
        {
            Wallet wallet = new Wallet(Guid.NewGuid());
            wallet.Deposite(new ValueObjects.Money(50));
            Assert.Throws<MoneyValidationException>(() => wallet.Withdraw(new ValueObjects.Money(51)));
        }

        [Fact]
        public void Withdraw_Should_Throw_MoneyValidationException_When_Currency_Is_Differ_Than_Balence_Currency()
        {
            Wallet wallet = new Wallet(Guid.NewGuid());
            Assert.Throws<MoneyValidationException>(() => wallet.Withdraw(new ValueObjects.Money(100, CurrencyEnum.Euro)));
        }

        [Fact]
        public void Withdraw_Should_Decrease_Balence_Correctly()
        {
            Wallet wallet = new Wallet(Guid.NewGuid());
            wallet.Deposite(new ValueObjects.Money(100));
            decimal prev_Amount = 100;
            wallet.Withdraw(new ValueObjects.Money(50));
            decimal after_Amount = 100 - 50;
            Assert.Equal(prev_Amount - after_Amount, wallet.Balance.Amount);
        }

        [Fact]
        public void Withdraw_Should_Create_PaymentTransaction_And_Add_It_To_Transactions_List()
        {
            Wallet wallet = new Wallet(Guid.NewGuid());
            wallet.Deposite(new ValueObjects.Money(100));
            decimal prev_Balence_Amount = 100;
            PaymentTransaction transaction = wallet.Withdraw(new ValueObjects.Money(50));
            decimal new_Balence_Amount = 100 - 50;

            Assert.NotNull(transaction);
            Assert.Equal(prev_Balence_Amount, transaction.PreviousBalance.Amount);
            Assert.Equal(new_Balence_Amount, transaction.CurrentBalance.Amount);
            Assert.Equal(wallet.Id, transaction.WalletId);
            Assert.Equal(TransactionTypeEnum.Withdraw, transaction.TransactionType);

            PaymentTransaction? find = wallet.Transactions.FirstOrDefault(x => x.Id == transaction.Id);

            Assert.NotNull(find);
            Assert.Equal(transaction.PreviousBalance.Amount, find.PreviousBalance.Amount);
            Assert.Equal(transaction.PreviousBalance.Currency, find.PreviousBalance.Currency);
            Assert.Equal(transaction.CurrentBalance.Amount, find.CurrentBalance.Amount);
            Assert.Equal(transaction.CurrentBalance.Currency, find.CurrentBalance.Currency);
            Assert.Equal(transaction.Id, find.Id);
            Assert.Equal(transaction.WalletId, find.WalletId);
            Assert.Equal(transaction.TransactionType, find.TransactionType);
        }
    }
}
