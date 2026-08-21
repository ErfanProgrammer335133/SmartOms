using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Invoice
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public List<InvoiceItem> Items { get; private set; }
        public Money FinalPrice { get; private set; }
        public Money TotalPrice { get; private set; }

        public Invoice(Guid orderId , List<InvoiceItem> items)
        {
            if (items == null || items.Count == 0)
                throw new DomainValidationException("هیچ ایتم خریدی برای فاکتور ثبت نشده .");
            Id = Guid.NewGuid();
            OrderId = orderId;
            Items = items;
            FinalPrice = new Money(Items.Sum(x => x.FinalPrice.Amount), Items.First().UnitPrice.Currency);
            TotalPrice = new Money(Items.Sum(x => x.TotalPrice.Amount), Items.First().UnitPrice.Currency);
        }

        public static Invoice CreateInvoice(Guid orderId , IReadOnlyCollection<CartItem> cartItems)
        {
            if (cartItems == null || cartItems.Count == 0)
                throw new DomainValidationException("لیست ورودی نا معتبر است");
            List<InvoiceItem> items = cartItems.Select(x => new InvoiceItem
            (
                x.ServiceId,
                x.Quantity,
                x.Title,
                x.UnitPrice,
                x.DiscountPercent
            )).ToList();

            return new Invoice(orderId, items);
        }
    }
}
