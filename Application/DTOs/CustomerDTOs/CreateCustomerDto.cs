using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CustomerDTOs
{
    public class CreateCustomerDto
    {
        public Guid UserId { get; set; }
        public required string FullName { get; set; }
        public string? Email { get; set; }
    }
}
