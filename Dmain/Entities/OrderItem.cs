using Dmain.Exceptions;
using Dmain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.Entities
{
    public class OrderItem
    {
        public Guid Id { get;  private set; }
        public Guid ServiceId { get; private set; }
        public string Title { get; private set; }
        public Money Price { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public OrderItem(Guid serviceId , string title , Money price)
        {
            if (price == null || string.IsNullOrWhiteSpace(title))
                throw new DomainValidationException("سرویس مورد نظر وجود ندارد.");
            Price = price;
            ServiceId = serviceId;
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }
    }
}
