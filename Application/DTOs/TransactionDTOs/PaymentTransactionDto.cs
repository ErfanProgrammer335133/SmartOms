using Domain.Enums;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.TransactionDTOs
{
    public class PaymentTransactionDto
    {
        public required Guid Id { get; set; }
        public required Guid WalletId { get; set; }
        public required string Amount { get; set; }
        public required string PreviousBalance { get; set; }
        public required string CurrentBalance { get; set; }
        public required string TransactionType { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
