using Domain.Enums;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PaymentDTOs
{
    public class PayDto
    {
        public Guid WalletId { get; set; }
        public Money Amount { get; set; }
        public Money PreviousBalence { get; set; }
        public Money CurrentBalence { get; set; }
        public TransactionTypeEnum Type { get; set; }
    }
}
