using Application.ApplicationGuard;
using Application.DTOs.UserDTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
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
        private readonly IPasswordHasher _hasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtservice _jwtService;

        public AuthService(IUserRepository userRepository , IPasswordHasher passwordHasher 
            , IUnitOfWork unitOfWork , IJwtservice jwtservice)
        {
            _userRepository = userRepository;
            _hasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _jwtService = jwtservice;
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

        public async Task<Result<LoginResultDto>> LotginAsync(LoginDto model)
        {
            return await ServiceHelper.Do<LoginResultDto>(async () =>
            {
                if (model is null)
                    return Result<LoginResultDto>.Failure("ورودی نا معتبر است .");
                User? user = await _userRepository.GetByUsernameAsync(model.UserName);
                if (user is null)
                    return Result<LoginResultDto>.Failure("نام کاربری یا کلمه عبور اشتباه است .");
                string hashPassword = _hasher.HashPassword(model.Password);
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
                    RefreshToken = refreshToken , 
                    AccessToken = accessToken , 
                    ExpireIn = 3600
                });
            
            }, 2);
        }

        public Task<Result<LoginResultDto>> LotginWithMobileAsync(LoginWithMobileDto model)
        {
            throw new NotImplementedException();
        }

        public Task<Result<RefreshTokenRespondDto>> RefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }


        ///////////////////////////////////////////////////////////////////////
        /// Helper Methods
        //////////////////////////////////////////////////////////////////////
        
        private async Task<Result> Register(RegisterDto model ,RoleEnum role)
        {
            if (model is null
                   || string.IsNullOrWhiteSpace(model.Username)
                   || string.IsNullOrWhiteSpace(model.Phone)
                   || string.IsNullOrWhiteSpace(model.Password)
               )
                return Result.Faliure("ورودی نا معتبر است");

            User? isExist = await _userRepository.GetByMobileOrUsernameAsync(model.Username, model.Phone);
            if (isExist is not null)
                return Result.Faliure("چنین کاربری با این نام کاربری یا شماره تلفن قبلا ثبت شده است .");
            string hashedPassword = _hasher.HashPassword(model.Password);
            User user = new User
            (
                username: model.Username,
                phone: model.Phone,
                hashPassword: hashedPassword,
                role: role
            );

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();

            return Result.Success();
        }

        private string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }
}
