using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CartDTOs
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public List<CartItemDto> Items { get; set; }
        public Money FinalPrice { get; set; }
        public Money TotalPrice { get; set; }

    }
}
