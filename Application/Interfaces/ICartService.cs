using Application.ApplicationGuard;
using Application.DTOs.CartDTOs;
using Application.DTOs.OrderDTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICartService
    {
        Task<Result<CartDto>> GetNewPricesAsync(Guid cartId);
        Task<Result> AddToCartAsync(CartItemDto item , Guid customerId);
        Task<Result> RemoveFromCartAsync(Guid customerId, Guid cartItemId);
        Task<Result> ClearCartAsync(Guid CartId);
        Task<Result<OrderDto>> CheckoutAsync(Guid customerId);
    }
}
