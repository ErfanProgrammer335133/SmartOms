using Application.ApplicationGuard;
using Application.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterDto model);
        Task<Result> RegisterAdminAsync(RegisterDto model);
        Task<Result<LoginResultDto>> LoginAsync(LoginDto model);
        Task<Result<LoginResultDto>> LoginWithMobileAsync(LoginWithMobileDto model);
        Task<Result<RefreshTokenRespondDto>> RefreshTokenAsync(string refreshToken);
    }
}
