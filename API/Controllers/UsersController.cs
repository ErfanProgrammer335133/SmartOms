using Application.ApplicationGuard;
using Application.DTOs.UserDTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        public UsersController(IUserService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Login([FromRoute] Guid id)
        {
            Guid claimId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (claimId != id)
            {
                ModelState.AddModelError("Forbiden" , "شما مجوز حذف حساب کاربر دیگری را ندارید .");
                return StatusCode(403, ModelState);
            }

            try
            {
                Result<UserDto> result = await _service.DeActivateUserAsync(id);
                if (!result.IsSuccess)
                    return NotFound(result);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "خطای سیستمی رخ داده است .");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public async Task<IActionResult> Admins()
        {
            string claimRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (claimRole != "Admin")
            {
                ModelState.AddModelError("Forbiden", "شما مجوز دسترسی به لیست ادمین هارا ندارید .");
                return StatusCode(403, ModelState);
            }

            try
            {
                Result<List<UserDto>> result = _service.GetAdminsList();
                if (!result.IsSuccess)
                    return NotFound(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if(Guid.TryParse(userId , out Guid guid)){
                if(guid != id && userRole != "Admin")
                {
                    ModelState.AddModelError("", "شما اجازه مشاهده اطلاعات کاربر دیگری را ندارید .");
                    return StatusCode(403, ModelState);
                }
            }
            else
            {
                ModelState.AddModelError("", "شناسه وارد شده معتبر نمیباشد .");
                return BadRequest(ModelState);
            }

            try
            {
                Result<UserDto> result = await _service.GetUserByIdAsync(id);
                if (!result.IsSuccess)
                    return NotFound(result);
                return Ok(result);
            }
                catch (Exception ex)
            {
                    return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserByUsername([FromQuery] string username)
        {
            string? claimUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            string? userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.IsNullOrEmpty(claimUsername))
            {
                if (username != claimUsername && userRole != "Admin")
                {
                    ModelState.AddModelError("", "شما اجازه مشاهده اطلاعات کاربر دیگری را ندارید .");
                    return StatusCode(403, ModelState);
                }
            }
            else
            {
                ModelState.AddModelError("", "نام کاربری وارد شده معتبر نمیباشد .");
                return BadRequest(ModelState);
            }

            try
            {
                Result<UserDto> result = await _service.GetUserByUsernameAsync(username);
                if (!result.IsSuccess)
                    return NotFound(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            string claimRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (claimRole != "Admin")
            {
                ModelState.AddModelError("Forbiden", "شما مجوز دسترسی به لیست کاربران را ندارید .");
                return StatusCode(403, ModelState);
            }

            try
            {
                Result<List<UserDto>> result = _service.GetUsersList();
                if (!result.IsSuccess)
                    return NotFound(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
