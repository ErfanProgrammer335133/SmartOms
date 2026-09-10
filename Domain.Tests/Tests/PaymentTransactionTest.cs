using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests.Tests
{
    public class PaymentTransactionTest
    {
        [Fact]
        public void Constructor_Should_Throw_MoneyValidationException_When_Amount_Is_Null()
        {
            Assert.Throws<MoneyValidationException>(() =>
            new PaymentTransaction(Guid.NewGuid(), null!, TransactionTypeEnum.Deposite, new Money(100), new Money(120)));
        }
        
        [Fact]
        public void Constructor_Should_Throw_MoneyValidationException_When_PreviousBalence_Is_Null()
        {
            Assert.Throws<MoneyValidationException>(() =>
            new PaymentTransaction(Guid.NewGuid(), new Money(80), TransactionTypeEnum.Deposite, null!, new Money(120)));
        }
        
        [Fact]
        public void Constructor_Should_Throw_MoneyValidationException_When_CurrentBalence_Is_Null()
        {
            Assert.Throws<MoneyValidationException>(() =>
            new PaymentTransaction(Guid.NewGuid(), new Money(80), TransactionTypeEnum.Deposite, new Money(100), null!));
        }

        [Fact]
        public void Constructor_Should_Create_PaymentTransaction_Correctly_When_All_Parameters_Are_Valid()
        {
            Guid walletId = Guid.NewGuid();
            Money amount = new Money(20);
            Money prevBalence = new Money(100);
            Money currentBalence = new Money(120);
            TransactionTypeEnum type = TransactionTypeEnum.Deposite;

            PaymentTransaction payment = new PaymentTransaction(walletId, amount, type, prevBalence, currentBalence);

            Assert.NotNull(payment);
            Assert.Equal(walletId, payment.WalletId);
            Assert.Equal(amount.Amount, payment.Amount.Amount);
            Assert.Equal(amount.Currency, payment.Amount.Currency);
            Assert.Equal(prevBalence.Amount, payment.PreviousBalance.Amount);
            Assert.Equal(prevBalence.Currency, payment.PreviousBalance.Currency);
            Assert.Equal(currentBalence.Amount, payment.CurrentBalance.Amount);
            Assert.Equal(currentBalence.Currency, payment.CurrentBalance.Currency);
            Assert.Equal(type, payment.TransactionType);
            Assert.Equal(DateTime.UtcNow.Year, payment.CreatedAt.Year);
            Assert.Equal(DateTime.UtcNow.Month, payment.CreatedAt.Month);
            Assert.Equal(DateTime.UtcNow.Day, payment.CreatedAt.Day);

        }
    }
}
