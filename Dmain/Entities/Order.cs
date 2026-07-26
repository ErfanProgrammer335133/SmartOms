using Dmain.Enums;
using Dmain.Exceptions;
using Dmain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }

        private readonly List<OrderItem> _items;
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Order(Guid customerId, List<OrderItem> items)
        {
            if (items == null)
                throw new DomainValidationException("لیست نامعتبر است .");
            _items = new List<OrderItem>(items);
            CustomerId = customerId;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public Money TotalPrice =>
            new Money(
                _items.Sum(x => x.Price.Amount),
                CurrencyEnum.Toman);

        public void MarkAsPaid()
        {
            if (Status != OrderStatus.Pending)
                throw new OrderStatusException("وضعیت نا معتبر");
            Status = OrderStatus.Paid;
        }

        public void AddItem(OrderItem item)
        {
            if (Status != OrderStatus.Pending)
                throw new OrderStatusException("بعد از پرداخت نمیتوان سرویس جدیدی اضافه کرد.");
            if (item == null)
                throw new DomainValidationException("سرویس اضافه شده معتبر نیست .");
            _items.Add(item);
        }

        public void RemoveItem(Guid itemId)
        {
            if (Status != OrderStatus.Pending)
                throw new OrderStatusException("بعد از پرداخت نمیتوان سرویسی را حذف کرد.");
            OrderItem? item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new DomainValidationException("چنین ایتمی یافت نشد .");
            _items.Remove(item);
        }
        public void ToInprogress()
        {
            if (Status == OrderStatus.InProgress)
                throw new OrderStatusException("هم اکنون سفارش در وضعیت در حال انجام است.");
            if (Status != OrderStatus.Paid)
                throw new OrderStatusException("نمیتوان از وضعیت فعلی به وضعیت درحال انجام رفت .");
            Status = OrderStatus.InProgress;
        }
        public void Complete()
        {
            if (Status == OrderStatus.Completed)
                throw new OrderStatusException("هم اکنون سفارش در وضعیت تکمیل شده است.");
            if (Status == OrderStatus.Canceled)
                throw new OrderStatusException("این سفارش قبلا کنسل شده و نمیتواند به تکمیل شده تغییر پیدا کند");
            if (Status != OrderStatus.InProgress)
                throw new OrderStatusException("نمیتوان از وضعیت فعلی به وضعیت تکمیل شده رفت .");
            Status = OrderStatus.Completed;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Completed)
                throw new OrderStatusException("نمی توان سفارشی که تکمیل شده را کنسل کرد.");
            if (Status == OrderStatus.Canceled)
                throw new OrderStatusException("این سفارش قبلا کنسل شده است .");
            Status = OrderStatus.Canceled;
        }

        public IReadOnlyCollection<OrderItem> GetItems() => _items.AsReadOnly();
        public bool IsCompleted() => Status == OrderStatus.Completed;
        public bool IsCanceled() => Status == OrderStatus.Canceled;
    }
}
