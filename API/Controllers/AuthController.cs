using Application.ApplicationGuard;
using Application.DTOs.UserDTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (model is null)
            {
                ModelState.AddModelError("" ,"ورودی نا معتبر است .");
                return BadRequest(ModelState);
            }

            try
            {
                Result<LoginResultDto> result = await _authService.LoginAsync(model);
                if (!result.IsSuccess)
                    return NotFound(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500 , ex.Message);
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (model is null)
            {
                ModelState.AddModelError("", "ورودی نامعتبر است.");
                return BadRequest(ModelState);
            }

            try
            {
                Result result = await _authService.RegisterAsync(   model);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok("ثبت نام با موفقیت انجام شد.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message);
            }
        }

        [HttpPost("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto model)
        {
            if (model is null)
            {
                ModelState.AddModelError("", "ورودی نامعتبر است.");
                return BadRequest(ModelState);
            }

            try
            {
                Result result = await _authService.RegisterAdminAsync(model);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok("ثبت نام با موفقیت انجام شد.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message);
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> LoginWithMobile([FromBody] LoginWithMobileDto model)
        {
            if (model is null)
            {
                ModelState.AddModelError("", "ورودی نا معتبر است .");
                return BadRequest(ModelState);
            }

            try
            {
                Result<LoginResultDto> result = await _authService.LoginWithMobileAsync(model);
                if (!result.IsSuccess)
                    return NotFound(result);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "خطای سیستمی رخ داده است .");
            }
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                ModelState.AddModelError("", "ورودی نا معتبر است .");
                return BadRequest(ModelState);
            }

            try
            {
                Result<RefreshTokenRespondDto> result = await _authService.RefreshTokenAsync(refreshToken);
                if (!result.IsSuccess)
                    return NotFound(result);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "خطای سیستمی رخ داده است .");
            }
        }

    }
}
