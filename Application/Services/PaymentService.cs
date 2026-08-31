using Application.DTOs.PaymentDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        public async Task<PaymentTransaction> PayAsync(PayDto dto)
        {
            return new PaymentTransaction(dto.WalletId , dto.Amount , dto.Type , dto.PreviousBalence , dto.CurrentBalence);
        }
    }
}
