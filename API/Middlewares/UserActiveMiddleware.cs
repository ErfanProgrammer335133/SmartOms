using Domain.Entities;
using Domain.Repositories;
using System.Security.Claims;

namespace API.Middlewares
{
    public class UserActiveMiddleware
    {
        private readonly RequestDelegate _next;
        public UserActiveMiddleware(RequestDelegate next)
        {
            _next = next;   
        }

        public async Task InvokeAsync(HttpContext context , IUserRepository repo)
        {
            if(context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if(Guid.TryParse(userId , out Guid id))
                {
                    User? user = await repo.GetByIdAsync(id);
                    if(user is null || !user.IsActive)
                    {
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "حساب کاربری شما غیر فعال شده است ."
                        });
                        return;
                    }
                }
            }
            await _next(context);
        }
    }
}
