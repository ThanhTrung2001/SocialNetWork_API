using EV.Common.Helpers.Authentication;
using Microsoft.AspNetCore.Http;

namespace EV.Common.Middlewares
{
    public class TokenExpiredCheckingMiddleware
    {
        private readonly RequestDelegate _next;
        public TokenExpiredCheckingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            //Split the bearer out of the token
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token) && JWTHelper.IsTokenExpired(token))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Token has expired.");
                return;
            }

            await _next(context);
        }
    }
}
