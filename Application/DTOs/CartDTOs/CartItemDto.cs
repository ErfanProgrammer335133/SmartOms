using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CartDTOs
{
    public class CartItemDto
    {
        public Guid ServiceId { get; set; }
        public string Title { get; set; }
        public Money UnitPrice { get; set; }
        public Money TotalPrice { get; set; }
        public int Quantity { get; set; }
        public bool IsChanged { get; set; }
        public decimal DiscountPercent { get; set; }

        public CartItemDto(Guid serviceId , string title , Money unitPrice 
            , int quantity , bool isChanged , decimal discountPercent)
        {
            ServiceId = serviceId;
            Title = title;
            UnitPrice = unitPrice;
            Quantity = quantity;
            IsChanged = isChanged;
            DiscountPercent = discountPercent;
            TotalPrice = new Money(Quantity * UnitPrice.Amount , UnitPrice.Currency);
        }
    }
}
