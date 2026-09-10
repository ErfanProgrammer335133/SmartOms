using Application.ApplicationGuard;
using Application.DTOs.UserDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<object> _hasher;
        public UserService(IUserRepository userRepository , IUnitOfWork unitOfWork , IPasswordHasher<object> hasher)
        {
            _userRepo = userRepository;
            _unitOfWork = unitOfWork;
            _hasher = hasher;
        }

        public async Task<Result<UserDto>> DeActivateUserAsync(Guid id)
        {
            return await ServiceHelper.Do<UserDto>(async () =>
            {
                User? user = await _userRepo.GetByIdAsync(id);
                if (user is null)
                    return Result<UserDto>.Failure("کاربری با این شناسه یافت نشد .");
                UserDto userDto = new UserDto
                {
                    Username = user.Username , 
                    Phone = user.Phone.PhoneNumber ,
                    HashPassword = user.HashPassword , 
                    Role = user.Role == Domain.Enums.RoleEnum.Customer ? "Customer" : "Admin"
                };

                user.DeActivate();
                await _unitOfWork.SaveAsync();
                return Result<UserDto>.Success(userDto);
            } , 2);
        }

        public Result<List<UserDto>> GetAdminsList()
        {
            return Result<List<UserDto>>.Success(_userRepo
                .GetUsersByCondition(x => x.Role == Domain.Enums.RoleEnum.Admin)
                .Select(x => new UserDto
                {
                    Username = x.Username,
                    HashPassword = x.HashPassword,
                    Phone = x.Phone.ToLocal(x.Phone.PhoneNumber),
                    Role = x.Role == Domain.Enums.RoleEnum.Customer ? "Customer" : "Admin"
                })
                .ToList()); 
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(Guid id)
        {
            return await ServiceHelper.Do<UserDto>(async () =>
            {
                User? user = await ServiceHelper.GetEntityFromRepo
                    (async () => await _userRepo.GetByIdAsync(id) , "کاربری با این شناسه یافت نشد .");
                return Result<UserDto>.Success(new UserDto
                {
                    Username = user.Username , 
                    HashPassword = user.HashPassword , 
                    Phone = user.Phone.ToLocal(user.Phone.PhoneNumber) , 
                    Role = user.Role == Domain.Enums.RoleEnum.Customer ? "Customer" : "Admin"
                });
            } , 2);
        }

        public async Task<Result<UserDto>> GetUserByUsernameAsync(string username)
        {
            return await ServiceHelper.Do<UserDto>(async () =>
            {
                User? user = await ServiceHelper.GetEntityFromRepo
                    (async () => await _userRepo.GetByUsernameAsync(username), "کاربری با این نام کاربری یافت نشد .");
                return Result<UserDto>.Success(new UserDto
                {
                    Username = user.Username,
                    HashPassword = user.HashPassword,
                    Phone = user.Phone.ToLocal(user.Phone.PhoneNumber),
                    Role = user.Role == Domain.Enums.RoleEnum.Customer ? "Customer" : "Admin"
                });
            }, 2);
        }

        public Result<List<UserDto>> GetUsersList()
        {
            return Result<List<UserDto>>.Success(_userRepo
                .GetUsersByCondition(x => true)
                .Select(x => new UserDto
                {
                    Username = x.Username,
                    HashPassword = x.HashPassword,
                    Phone = x.Phone.ToLocal(x.Phone.PhoneNumber),
                    Role = x.Role == Domain.Enums.RoleEnum.Customer ? "Customer" : "Admin"
                })
                .ToList());
        }

        public Result<List<UserDto>> SearchUsers(string title)
        {
            return Result<List<UserDto>>.Success(_userRepo
                .GetUsersByCondition(x => x.Username.Contains(title))
                .Select(x => new UserDto
                {
                    Username = x.Username,
                    HashPassword = x.HashPassword,
                    Phone = x.Phone.ToLocal(x.Phone.PhoneNumber),
                    Role = x.Role == Domain.Enums.RoleEnum.Customer ? "Customer" : "Admin"
                })
                .ToList());
        }

        public async Task<Result> UpdateUserAsync(UpdateUserDto model)
        {
            return await ServiceHelper.Do(async () =>
            {
                if (model is null)
                    return Result.Faliure("ورودی نا معتبر .");
                User? user = await ServiceHelper.GetEntityFromRepo<User>
                    (async () => await _userRepo.GetByIdAsync(model.Id), "کاربری با این شناسه یافت نشد .");
                user.SetUsername(model.Username);
                user.SetPhone(model.Phone);
                string hashedPassword = _hasher.HashPassword(user, model.Password);
                user.SetPassword(hashedPassword);

                await _unitOfWork.SaveAsync();
                return Result.Success();
            }, 2);
        }
    }
}
