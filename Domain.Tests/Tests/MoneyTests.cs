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
    public class MoneyTests
    {
        private void Check_Constructor(Money money , decimal amount , CurrencyEnum currency)
        {
            Assert.NotNull(money);
            Assert.Equal(amount, money.Amount);
            Assert.Equal(currency, money.Currency);
        }

        [Fact]
        public void Constructor_Shold_Throw_MoneyValidationExxeptin_When_Amout_Is_Less_Than0()
        {
            Assert.Throws<MoneyValidationException>(() => new Money(-1));
            Assert.Throws<MoneyValidationException>(() => new Money(-1, Enums.CurrencyEnum.Dollar));
        }

        [Fact]
        public void Constructor_Shold_Create_Money_Correctly_When_Amount_Is_Valid_Without_CUrrency()
        {
            Check_Constructor(new Money(100), 100, CurrencyEnum.Toman);
        }
        [Fact]
        public void Constructor_Shold_Create_Money_Correctly_When_Amount_Is_Valid_With_CUrrency()
        {
            Check_Constructor(new Money(100 , CurrencyEnum.Pound), 100, CurrencyEnum.Pound);
        }
    }
}
