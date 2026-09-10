using Application.ApplicationGuard;
using Application.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<Result> UpdateUserAsync(UpdateUserDto model);
        Task<Result<UserDto>> DeActivateUserAsync(Guid id);
        Result<List<UserDto>> GetUsersList();
        Result<List<UserDto>> GetAdminsList();
        Task<Result<UserDto>> GetUserByIdAsync(Guid id);
        Task<Result<UserDto>> GetUserByUsernameAsync(string username);
        Result<List<UserDto>> SearchUsers(string title);
    }
}
