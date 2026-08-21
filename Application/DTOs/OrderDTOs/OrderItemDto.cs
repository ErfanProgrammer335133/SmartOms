using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OrderDTOs
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get;  set; }
        public string Title { get;  set; }
        public int Quantity { get;  set; }
        public Money UnitPrice { get;  set; }
        public DateTime CreatedAt { get; set; }
    }
}
