using Dmain.Exceptions;
using Dmain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmain.Entities
{
    public class CartItem
    {
        public Guid Id { get; private set; }
        public Guid ServiceId { get; private set; }
        public string Title { get; private set; }
        public Money Price { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public CartItem(Guid serviceId , string title , Money price)
        {
            if (price == null)
                throw new DomainValidationException("قیمت نا معتبر .");
            Id = Guid.NewGuid();
            ServiceId = serviceId;
            Title = title;
            Price = price;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
