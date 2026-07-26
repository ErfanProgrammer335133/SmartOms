using Dmain.Exceptions;
using Dmain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.Entities
{
    public class Cart
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }

        private readonly List<CartItem> _items = new();

        public Cart(Guid customerId)
        {
            CustomerId = customerId;
        }

        public void AddItem(BusinessService service)
        {
            if (service == null)
                throw new DomainValidationException("سرویس نا معتبر است .");
            _items.Add(new CartItem(service.Id , service.Title , service.Price));
        }

        public void RemoveItem(Guid itemId)
        {
            CartItem? item = _items.FirstOrDefault(c => c.Id == itemId);
            if (item == null)
                throw new DomainValidationException("چنین سرویسی یافت نشد");
            _items.Remove(item);
        }

        public Order CheckOut()
        {
            if (!_items.Any())
                throw new DomainValidationException("هیچ ایتمی در سبد خربد برای ثبت سفارش وجود ندارد .");
            List<OrderItem> orderItems = _items.Select(x => new OrderItem(
                x.ServiceId , 
                x.Title , 
                x.Price
                )
            ).ToList();
            return new Order(CustomerId, orderItems);
        }

        public void Clear()
        {
            _items.Clear();
        }
        public Money TotalPrice()
        {
            decimal amount = _items.Sum(x => x.Price.Amount);
            return new Money(amount, _items.First().Price.Currency);
        }
    }
}
