using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.BusinessServviceDTOs
{
    public class UpdateServiceDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Explanation { get; set; }
        public Money Price { get; set; }
    }
}
