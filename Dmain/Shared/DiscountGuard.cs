using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shared
{
    public static class DiscountGuard
    {
        public static void CheckDiscount(decimal percent)
        {
            if (percent < 0 || percent > 100)
                throw new DomainValidationException("مقدار تخفیف نمیتواند منفی یا بیش از 100 درصد باشد .");
        }
        public static Money CalculateDiscount(decimal amount, decimal percent)
            => new Money(amount - (amount * percent) / 100);
        
        public static Money CalculateDiscount(Money amount, decimal percent)
            => new Money(amount.Amount - (amount.Amount * percent) / 100 , amount.Currency);
    }
}
