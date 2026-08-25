using Application.ApplicationGuard;
using Application.DTOs.CustomerDTOs;
using Application.DTOs.UserDTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.AspNet.Identity;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerService _customerService;
        private readonly IPasswordHasher _hasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtservice _jwtService;

        public AuthService(IUserRepository userRepository , IPasswordHasher passwordHasher 
            , IUnitOfWork unitOfWork , IJwtservice jwtservice , ICustomerService customerServcie)
        {
            _userRepository = userRepository;
            _hasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _jwtService = jwtservice;
            _customerService = customerServcie;
        }
        public async Task<Result> RegisterAsync(RegisterDto model)
        {
            return await ServiceHelper.Do(async () =>
            {
                await Register(model, RoleEnum.Customer);
                return Result.Faliure("خطایی رخ داده است");

            }, 2);
        }

        public async Task<Result> RegisterAdminAsync(RegisterDto model)
        {
            return await ServiceHelper.Do(async () =>
            {
                await Register(model, RoleEnum.Admin);
                return Result.Faliure("خطایی رخ داده است");
            }, 2);
        }

        public async Task<Result<LoginResultDto>> LoginAsync(LoginDto model)
        {
            return await ServiceHelper.Do<LoginResultDto>(async () =>
            {
                if (model is null)
                    return Result<LoginResultDto>.Failure("ورودی نا معتبر است .");
                return await Login(async () => await _userRepository.GetByUsernameAsync(model.UserName), model.Password);
            }, 2);
        }

        public async Task<Result<LoginResultDto>> LoginWithMobileAsync(LoginWithMobileDto model)
        {
            return await ServiceHelper.Do<LoginResultDto>(async () =>
            {
                if (model is null)
                    return Result<LoginResultDto>.Failure("ورودی نا معتبر است .");
                return await Login(async () => await _userRepository.GetByMobileAsync(model.Phone), model.Password);
            }, 2);
        }

        public async Task<Result<RefreshTokenRespondDto>> RefreshTokenAsync(string refreshToken)
        {
            return await ServiceHelper.Do<RefreshTokenRespondDto>(async () =>
            {
                User? user = await _userRepository.GetByRefreshToken(refreshToken);
                if (user is null)
                    return Result<RefreshTokenRespondDto>.Failure("کاربر مورد نظر یافت نشد .");

                if(DateTime.UtcNow > user.Expiry)
                    return Result<RefreshTokenRespondDto>.Failure("ورود ناموفق بود لطفا لاگین کنید.");

                string accessToken = _jwtService.GenerateToken(user);
                Console.WriteLine("This is access token : " + accessToken);
                string newRefreshToken = GenerateRefreshToken();
                DateTime expiry = DateTime.UtcNow.AddDays(20);

                user.SetRefreshToken(newRefreshToken , expiry);
                await _unitOfWork.SaveAsync();

                return Result<RefreshTokenRespondDto>.Success(new RefreshTokenRespondDto
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken,
                    ExpireIn = 3600
                });

            } , 2);
        }


        ///////////////////////////////////////////////////////////////////////
        /// Helper Methods
        //////////////////////////////////////////////////////////////////////
        
        private async Task<Result<RegisterResultDto>> Register(RegisterDto model ,RoleEnum role)
        {
            return await ServiceHelper.Do<RegisterResultDto>(async () =>
            {
                if (model is null
                   || string.IsNullOrWhiteSpace(model.Username)
                   || string.IsNullOrWhiteSpace(model.Phone)
                   || string.IsNullOrWhiteSpace(model.Password)
               )
                    return Result<RegisterResultDto>.Failure("ورودی نا معتبر است");

                User? isExist = await _userRepository.GetByMobileOrUsernameAsync(model.Username, model.Phone);
                if (isExist is not null)
                    return Result<RegisterResultDto>.Failure("چنین کاربری با این نام کاربری یا شماره تلفن قبلا ثبت شده است .");
                string hashedPassword = _hasher.HashPassword(model.Password);
                User user = new User
                (
                    username: model.Username,
                    phone: model.Phone,
                    hashPassword: hashedPassword,
                    role: role
                );
                await _userRepository.AddAsync(user);

                CustomerDto customer = await CreateAndVerifyCustomer(user.Id, model.FullName, model.Email);

                await _unitOfWork.SaveAsync();

                return Result<RegisterResultDto>.Success(new RegisterResultDto
                {
                    UserId = user.Id,
                    CustomerId = customer.Id,
                    Username = user.Username,
                    FullName = customer.FullName,
                    Email = customer.Email,
                    Phone = user.Phone.ToLocal(user.Phone.PhoneNumber)
                });
            } ,1);
        }

        private async Task<Result<LoginResultDto>> Login(Func<Task<User>> func , string password)
        {
            return await ServiceHelper.Do<LoginResultDto>(async () =>
            {
                User? user = await func();
                if (user is null)
                    return Result<LoginResultDto>.Failure("نام کاربری یا کلمه عبور اشتباه است .");
                string hashPassword = _hasher.HashPassword(password);
                var res = _hasher.VerifyHashedPassword(hashPassword, user.HashPassword);
                if (res == PasswordVerificationResult.Failed)
                {
                    Console.WriteLine("Hash error");
                    return Result<LoginResultDto>.Failure("نام کاربری یا کلمه عبور اشتباه است .");
                }

                string accessToken = _jwtService.GenerateToken(user);

                string refreshToken = GenerateRefreshToken();
                DateTime expiry = DateTime.UtcNow.AddDays(30);
                user.SetRefreshToken(refreshToken, expiry);

                await _unitOfWork.SaveAsync();
                return Result<LoginResultDto>.Success(new LoginResultDto
                {
                    RefreshToken = refreshToken,
                    AccessToken = accessToken,
                    ExpireIn = 3600
                });
            } , 1);
        }

        private string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<CustomerDto> CreateAndVerifyCustomer(Guid userId , string fullName , string email)
        {
            Result<CustomerDto> customerRes = await _customerService.CreateCustomer(new CreateCustomerDto
            {
                UserId = userId,
                FullName = fullName,
                Email = email
            });

            if (!customerRes.IsSuccess)
                throw new DomainValidationException(customerRes.ErrorMessage);
            return customerRes.Value;
        }

    }
}
