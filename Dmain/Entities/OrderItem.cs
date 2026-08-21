using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get;  private set; }
        public Guid ServiceId { get; private set; }
        public string Title { get; private set; }
        public int Quantity { get; private set; }
        public int MaxQuantity { get; private set; }
        public Money UnitPrice { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public OrderItem(Guid serviceId , string title , Money unitPrice , int quantity , int maxQuantity)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainValidationException("عنوان سرویس نباید خالی باشد.");
            if (unitPrice == null)
                throw new MoneyValidationException("قیمت نا معتبر است.");
            if (quantity <= 0 || quantity > maxQuantity)
                throw new DomainValidationException("تعداد نامعتبر .");
            Quantity = quantity;
            MaxQuantity = maxQuantity;
            UnitPrice = unitPrice;
            ServiceId = serviceId;
            Title = title;
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public Money TotalPrice => new Money(UnitPrice.Amount * Quantity , UnitPrice.Currency);
        public void IncreaseQuantity(int count)
        {
            if (count < 0)
                throw new DomainValidationException("تعداد نمی تواند منفی باشد .");
            if (Quantity + count > MaxQuantity)
                throw new DomainValidationException("به دلیل عبور از سقف تعداد مجاز ثبت سفارش امکان افزایش تعداد وجود ندارد.");
            Quantity += count;
        }
        public void DecreaseQuantity(int count)
        {
            if (count < 0)
                throw new DomainValidationException("تعداد نمی تواند منفی باشد .");
            if (Quantity - count <= 0)
                throw new DomainValidationException("تعداد کاهش مساوی یا بیشتر از کل موجودی ان است. ");
            Quantity -= count;
        }
    }
}
