using System.Text.Json;

namespace API.Middlewares
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(UnauthorizedAccessException)
            {
                context.Response.StatusCode = 403; // ❸ کد وضعیت را تنظیم می‌کند
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    error = "شما اجازه دسترسی به این بخش را ندارید.."
                }));
            }
        }
    }
}
