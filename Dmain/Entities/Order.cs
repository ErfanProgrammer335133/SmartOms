using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }

        private readonly List<OrderItem> _items;
        public OrderStatusEnum Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Order(Guid customerId, List<OrderItem> items)
        {
            if (items == null || items.Count == 0)
                throw new DomainValidationException("لیست نامعتبر است .");
            _items = new List<OrderItem>(items);
            CustomerId = customerId;
            Status = OrderStatusEnum.Pending;
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public Money TotalPrice =>
            new Money(
                _items.Sum(x => x.TotalPrice.Amount),
                _items.First().UnitPrice.Currency);

        public void AddItem(OrderItem item)
        {
            if (Status != OrderStatusEnum.Pending)
                throw new OrderStatusException("بعد از پرداخت نمیتوان سرویس جدیدی اضافه کرد.");
            if (item == null)
                throw new DomainValidationException("سرویس اضافه شده معتبر نیست .");
            OrderItem? isExisted = Items.FirstOrDefault
                (x => x.ServiceId == item.ServiceId && x.UnitPrice.Amount == item.UnitPrice.Amount);
            if (isExisted != null)
                isExisted.IncreaseQuantity(item.Quantity);
            else
                _items.Add(item);
        }

        public void RemoveItem(Guid itemId)
        {
            if (Status != OrderStatusEnum.Pending)
                throw new OrderStatusException("بعد از پرداخت نمیتوان سرویسی را حذف کرد.");
            OrderItem? item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new DomainValidationException("چنین ایتمی یافت نشد .");
            _items.Remove(item);
        }

        public void MarkAsPaid()
        {
            if (Status != OrderStatusEnum.Pending)
                throw new OrderStatusException("وضعیت نا معتبر");
            Status = OrderStatusEnum.Paid;
        }

        public void ToInprogress()
        {
            if (Status == OrderStatusEnum.InProgress)
                throw new OrderStatusException("هم اکنون سفارش در وضعیت در حال انجام است.");
            if (Status != OrderStatusEnum.Paid)
                throw new OrderStatusException("نمیتوان از وضعیت فعلی به وضعیت درحال انجام رفت .");
            Status = OrderStatusEnum.InProgress;
        }
        public void Complete()
        {
            if (Status == OrderStatusEnum.Completed)
                throw new OrderStatusException("هم اکنون سفارش در وضعیت تکمیل شده است.");
            if (Status == OrderStatusEnum.Canceled)
                throw new OrderStatusException("این سفارش قبلا کنسل شده و نمیتواند به تکمیل شده تغییر پیدا کند");
            if (Status != OrderStatusEnum.InProgress)
                throw new OrderStatusException("نمیتوان از وضعیت فعلی به وضعیت تکمیل شده رفت .");
            Status = OrderStatusEnum.Completed;
        }

        public void Cancel()
        {
            if (Status == OrderStatusEnum.Completed)
                throw new OrderStatusException("نمی توان سفارشی که تکمیل شده را کنسل کرد.");
            if (Status == OrderStatusEnum.Canceled)
                throw new OrderStatusException("این سفارش قبلا کنسل شده است .");
            Status = OrderStatusEnum.Canceled;
        }

        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public bool IsCompleted() => Status == OrderStatusEnum.Completed;
        public bool IsCanceled() => Status == OrderStatusEnum.Canceled;
    }
}
