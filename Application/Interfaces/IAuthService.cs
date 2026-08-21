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
        Task<Result<LoginResultDto>> LotginAsync(LoginDto model);
        Task<Result<LoginResultDto>> LotginWithMobileAsync(LoginWithMobileDto model);
        Task<Result<RefreshTokenRespondDto>> RefreshToken(string refreshToken);
    }
}
