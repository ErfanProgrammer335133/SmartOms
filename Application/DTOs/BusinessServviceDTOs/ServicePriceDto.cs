using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.BusinessServviceDTOs
{
    public class ServicePriceDto
    {
        public Guid ServiceId { get; set; }
        public Money Price { get; set; }
        public bool IsActive { get; set; }
    }
}
