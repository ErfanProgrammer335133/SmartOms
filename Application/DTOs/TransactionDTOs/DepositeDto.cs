using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.TransactionDTOs
{
    public class DepositeDto
    {
        public required Guid WalletId { get; set; }
        public required Guid CustomerId { get; set; }
        public required Money Amount { get; set; }
    }
}
