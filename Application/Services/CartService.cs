using Application.ApplicationGuard;
using Application.DTOs.CartDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Application.Exceptions;
using Domain.ValueObjects;
using Domain.IRepositories;
using Application.DTOs.BusinessServviceDTOs;
using Application.DTOs.OrderDTOs;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IInvoiceRepository _invoicetRepository;
        private readonly IBusinessServiceRepository _businessServiceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentTransactionRepository _transsactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBusinessserviceService _businessService;

        public CartService(
            ICartRepository cartRepository, ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork, IOrderRepository orderRepository
            , IPaymentTransactionRepository transactionRepository
            , IBusinessserviceService businessserviceService
            , IBusinessServiceRepository businessServiceRepository
            , IWalletRepository walletRepository
            , IInvoiceRepository invoiceRepository
            )

        {
            _cartRepository = cartRepository;
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _transsactionRepository = transactionRepository;
            _businessService = businessserviceService;
            _businessServiceRepository = businessServiceRepository;
            _walletRepository = walletRepository;
            _invoicetRepository = invoiceRepository;
        }

        public async Task<Result<CartDto>> GetNewPricesAsync(Guid cartId)
        {
            return await ServiceHelper.Do<CartDto>(async () =>
            {
                Cart? cart = await GetCartWithCartId(cartId);
                CartDto cartDto = new CartDto { CustomerId = cart.CustomerId, 
                    Id = cart.Id, Items = new(), FinalPrice = new Money(0), TotalPrice = new Money(0)
                };
                List<Guid> ServiceIds = GetServiceIds(cart.Items);
                List<ServicePriceDto> prices = await _businessService.GetPricesAsync(ServiceIds);
                if (prices == null)
                    return Result<CartDto>.Failure("سرویس های مورد نظر شما وجود ندارند یا غیر فعال هستند.");
                UpdatePrices(prices, cartDto, cart);
                if(cart.Items.Count != 0)
                {
                    cartDto.FinalPrice = new Money(cart.FinalPrice.Amount, cart.FinalPrice.Currency);
                    cartDto.TotalPrice = new Money(cart.TotalPrice.Amount, cart.TotalPrice.Currency);
                }
                await _unitOfWork.SaveAsync();
                return Result<CartDto>.Success(cartDto);
            }, 1);
        }

        public async Task<Result> AddToCartAsync(CartItemDto item, Guid customerId)
        {
            return await ServiceHelper.Do(async () =>
            {
                if (item == null)
                    return Result.Faliure("ورودی نا معتبر");

                Cart? cart = await _cartRepository.GetByCustomerIdAsync(customerId);
                if (cart == null)
                    cart = await CreateCart(customerId);

                BusinessService? service = await _businessServiceRepository.GetByIdAsync(item.ServiceId);
                CheckServiceConditions(service, item);

                cart?.AddItem(item.ServiceId, item.Title
                    , item.UnitPrice, item.Quantity, service.MaxQuantity, item.DiscountPercent);
                await _unitOfWork.SaveAsync();
                return Result.Success();
            }
            , 3);
        }

        public async Task<Result> RemoveFromCartAsync(Guid customerId, Guid cartItemId)
        {
            return await ServiceHelper.Do(async () =>
            {
                Cart? cart = await _cartRepository.GetByCustomerIdAsync(customerId);
                if (cart == null)
                    return Result.Faliure("سبد خرید پیدا نشد .");

                cart.RemoveItem(cartItemId);
                await _unitOfWork.SaveAsync();
                return Result.Success();
            },
            2);
        }

        public async Task<Result<OrderDto>> CheckoutAsync(Guid customerId)
        {
            Wallet wallet = new Wallet(customerId);
            Money amount = new Money(0);
            return await ServiceHelper.Do<OrderDto>(async () =>
            {
                await _unitOfWork.BeginTransactionAsync();

                Cart? cart = await GetCartWithCustomerId(customerId);

                wallet = await GetWallet(customerId);

                Order order = cart.CheckOut();
                await _orderRepository.AddAsync(order);
                PaymentTransaction payment = wallet.Withdraw(new Money(order.TotalPrice.Amount , order.TotalPrice.Currency));
                amount = new Money(order.TotalPrice.Amount, order.TotalPrice.Currency);
                await _transsactionRepository.AddAsync(payment);
                order.MarkAsPaid();

                Invoice invoice = Invoice.CreateInvoice(order.Id, cart.Items);
                await _invoicetRepository.AddAsync(invoice);

                OrderDto orderDto = ConvertToOrderDto(order, invoice);

                await _unitOfWork.SaveAsync();
                cart.Checouted();
                cart.Clear();
                await _unitOfWork.CommitAsync();
                return Result<OrderDto>.Success(orderDto);
            },
            2 , 
            () =>
            {
                return wallet.Deposite(amount);
            }
            );
        }

        public async Task<Result> ClearCartAsync(Guid cartId)
        {
            return await ServiceHelper.Do(async () =>
            {
                Cart cart = await GetCartWithCartId(cartId);
                cart.Clear();
                await _unitOfWork.SaveAsync();
                return Result.Success();
            } ,1);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Helper methods : 
        ///////////////////////////////////////////////////////////////////////////////////////

        private OrderDto ConvertToOrderDto(Order order, Invoice invoice)
        {
            OrderDto orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Items = order.Items
                    .Select(x => new OrderItemDto
                    {
                        Id = x.Id,
                        ServiceId = x.ServiceId,
                        Title = x.Title,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice,
                        CreatedAt = x.CreatedAt
                    }).ToList(),
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                Invoice = invoice
            };
            return orderDto;
        }

        private void UpdatePrices
            (List<ServicePriceDto> prices, CartDto cartDto, Cart cart)
        {
            foreach (CartItem item in cart.Items.ToList())
            {
                ServicePriceDto? latestPrice = prices.FirstOrDefault(x => x.ServiceId == item.ServiceId);
                if (latestPrice != null && latestPrice.IsActive)
                {
                    cartDto.Items.Add(new CartItemDto
                    (
                        serviceId: item.ServiceId,
                        title: item.Title,
                        quantity: item.Quantity,
                        unitPrice: latestPrice.Price,
                        isChanged: latestPrice.Price.Amount != item.UnitPrice.Amount ? true : false,
                        discountPercent: item.DiscountPercent
                    ));
                    item.SetUnitPrice(latestPrice.Price);
                }
                else
                    cart.RemoveItem(item.Id);
            }
        }

        private async Task<Cart?> GetCartWithCustomerId(Guid customerId)
        {
            return await ServiceHelper.GetEntityFromRepo
                (async () => await _cartRepository.GetByCustomerIdAsync(customerId), "سبد خرید یافت نشد .");
        }

        private async Task<Cart?> GetCartWithCartId(Guid cartId)
        {
            return await ServiceHelper.GetEntityFromRepo<Cart>
                (async () => await _cartRepository.GetByIdAsync(cartId), "سبد خرید یافت نشد .");
        }

        private async Task<Wallet?> GetWallet(Guid customerId)
        {
            return await ServiceHelper.GetEntityFromRepo<Wallet>
                (async () => await _walletRepository.GetByCustomerIdAsync(customerId), "کیف پول پیدا نشد .");
        }

        private List<Guid> GetServiceIds(IReadOnlyCollection<CartItem> items)
            => items.Select(x => x.ServiceId).ToList();

        private async Task<Cart> CreateCart(Guid customerId)
        {
            Customer? customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                throw new NotFoundException("مشتری یافت نشد");

            Cart cart = new Cart(customerId);
            await _cartRepository.AddAsync(cart);
            return cart;
        }

        private void CheckServiceConditions(BusinessService service , CartItemDto item)
        {
            if (service == null)
                throw new NotFoundException("سرویس یافت نشد");

            if (service.IsFree && item.TotalPrice.Amount != 0)
                throw new DomainValidationException("این یک سرویس رایگان است و نمی تواند شامل هزینه باشد .");

            if (service.Price.Amount != item.UnitPrice.Amount
            || service.Price.Currency != item.UnitPrice.Currency)
                throw new DomainValidationException("قیمت سرویس با قیمت ایتم وارد شده برابر نیست .");

            if (service.MaxQuantity < item.Quantity)
                throw new DomainValidationException("تعداد ایتم از حدااکثر تعداد مجاز سرویس بالا تر است .");
        }

    }
}
