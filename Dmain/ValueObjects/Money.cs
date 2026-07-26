using Dmain.Enums;
using Dmain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.ValueObjects
{
    public class Money
    {
        public decimal Amount { get; private set; }
        public CurrencyEnum Currency { get; private set; }
        public Money(decimal amount , CurrencyEnum currency)
        {
            if (amount < 0)
                throw new MoneyValidationException("مقدار پول نمی تواند منفی باشد");
            Amount = amount;
            Currency = currency;
        }
        public Money(decimal amount)
        {
            if (amount < 0)
                throw new MoneyValidationException("مقدار پول نمی تواند منفی باشد");
            Amount = amount;
            Currency = CurrencyEnum.Toman;
        }
    }
}
