using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OrderDTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public OrderStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Invoice Invoice { get; set; }

    }
}
