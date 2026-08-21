
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
    public class InvoiceItem
    {
        public Guid Id { get; private set; }
        public Guid ServiceId { get; private set; }
        public string ServiceTitle { get; private set; }
        public int Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
        public Money TotalPrice { get; private set; }
        public decimal DiscountPercent { get; private set; }
        public Money FinalPrice { get; private set; }
        public DateTime CreatedAt { get;  private set; }

        public InvoiceItem(Guid serviceId , int quantity ,string serviceTitle , Money unitPrice , decimal discountPercent)
        {
            if (quantity <= 0)
                throw new DomainValidationException("تعداد خرید در فاکتور نمی تواند منفی یا صفر باشد .");
            DiscountGuard.CheckDiscount(discountPercent);
            if (string.IsNullOrWhiteSpace(serviceTitle))
                throw new DomainValidationException("عنوان سرویس نمی تواند خالی باشد .");
            Id = Guid.NewGuid();
            ServiceId = serviceId;
            ServiceTitle = serviceTitle;
            UnitPrice = unitPrice;
            Quantity = quantity;
            DiscountPercent = discountPercent;
            TotalPrice = new Money(UnitPrice.Amount * quantity, UnitPrice.Currency);
            FinalPrice = DiscountGuard.CalculateDiscount(TotalPrice, discountPercent);
            CreatedAt = DateTime.UtcNow;
        }
    }
}
