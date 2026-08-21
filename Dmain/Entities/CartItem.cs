using Domain.Exceptions;
using Domain.Shared;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CartItem
    {
        public Guid Id { get; private set; }
        public Guid ServiceId { get; private set; }
        public string Title { get; private set; }
        public Money UnitPrice { get; private set; }
        public decimal DiscountPercent { get; private set; }
        public int Quantity { get; private set; }
        public int MaxQuantity { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public CartItem
            (Guid serviceId , string title , Money unitPrice , int quantity ,int maxQuantity , decimal discountPercent)
        {
            if (quantity <= 0 || quantity > maxQuantity)
                throw new DomainValidationException($"تعداد نمی تواند کوچک تر یا مساوی صفر یا بزرگ تر از باشد{maxQuantity}");
            if (discountPercent < 0 || discountPercent > 100)
                throw new DomainValidationException("مقدار تخفیف نمیتواند منفی یا بیش از 100 درصد باشد .");
            SetUnitPrice(unitPrice);
            Id = Guid.NewGuid();
            ServiceId = serviceId;
            Title = title;
            CreatedAt = DateTime.UtcNow;
            Quantity = quantity;
            MaxQuantity = maxQuantity;
            DiscountPercent = discountPercent;
        }

        public Money TotalPrice => new Money(UnitPrice.Amount * Quantity, UnitPrice.Currency);
        public Money FinalPrice => DiscountGuard.CalculateDiscount(TotalPrice , DiscountPercent);
        public void IncreaseQuantity(int count)
        {
            if(count < 0)
                throw new DomainValidationException("تعداد نمیتواند منفی باشد.");
            if (Quantity + count > MaxQuantity)
                throw new DomainValidationException("به دلیل عبور از سقف تعداد مجاز ثبت سفارش امکان افزایش تعداد وجود ندارد.");
            Quantity += count;
        }
        public void DecreaseQuantity(int count)
        {
            if (count < 0)
                throw new DomainValidationException("تعداد نمیتواند منفی باشد.");
            if (Quantity - count <= 0)
                throw new DomainValidationException("تعداد نمیتواند صفر شود .");
            Quantity -= count;
        }

        public void SetUnitPrice(Money price)
        {
            if (price == null)
                throw new MoneyValidationException("قیمت ورودی نا معتبر");
            UnitPrice = price;
        }
    }
}
