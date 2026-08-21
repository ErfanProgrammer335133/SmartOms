using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.BusinessServviceDTOs
{
    public class CreateServiceDto
    {
        public string Title { get; set; }
        public string Explanation { get; set; }
        public Money Price { get; set; }
        public int MaxQuantity { get; set; }
        public bool IsFree { get; set; }
    }
}
