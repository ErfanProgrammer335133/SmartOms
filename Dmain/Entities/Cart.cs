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
    public class Cart
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public bool IsCheckout { get; set; }

        private readonly List<CartItem> _items = new();

        public Cart(Guid customerId)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            IsCheckout = false;
        }

        public void AddItem(Guid ServiceId , string Titlle , Money Price , int quantity , int maxQuantity , decimal discountPecent)
        {
            if (string.IsNullOrWhiteSpace(Titlle) || Price == null)
                throw new DomainValidationException("سرویس نا معتبر است .");
            DiscountGuard.CheckDiscount(discountPecent);
            CartItem? isExist = _items.FirstOrDefault(x => x.ServiceId == ServiceId && x.UnitPrice.Amount == Price.Amount);
            if(isExist != null)
            {
                isExist.IncreaseQuantity(quantity);
                return;
            }

            CartItem item = new CartItem(ServiceId, Titlle, Price, quantity, maxQuantity , discountPecent);
            _items.Add(item);
        }

        public void RemoveItem(Guid itemId)
        {
            CartItem? item = _items.FirstOrDefault(c => c.Id == itemId);
            if (item == null)
                throw new NotFoundException("چنین ایتمی یافت نشد");
            _items.Remove(item);
        }

        public Order CheckOut()
        {
            if (!_items.Any())
                throw new DomainValidationException("هیچ ایتمی در سبد خربد برای ثبت سفارش وجود ندارد .");
            if (IsCheckout)
                throw new DomainValidationException("این سفارش قبلا ثبت سفارش شده است .");

            List<OrderItem> orderItems = _items.Select(x => new OrderItem(
                x.ServiceId , 
                x.Title , 
                DiscountGuard.CalculateDiscount(x.UnitPrice.Amount , x.DiscountPercent) , 
                x.Quantity , 
                x.MaxQuantity
                )
            ).ToList();
            return new Order(CustomerId, orderItems);
        }
     
        public void Clear()
        {
            _items.Clear();
            IsCheckout = false;
        }

        public void Checouted() => IsCheckout = true;
        public Money TotalPrice => new Money(_items.Sum(x => x.TotalPrice.Amount), _items.First().UnitPrice.Currency);
        public Money FinalPrice => new Money(_items.Sum(x => x.FinalPrice.Amount), _items.First().UnitPrice.Currency);

        public bool HasItems() => _items.Count > 0;
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();
    }
}
