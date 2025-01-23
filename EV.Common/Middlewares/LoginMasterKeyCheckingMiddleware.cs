using EV.Common.SettingConfigurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace EV.Common.Middlewares
{
    public class LoginMasterKeyCheckingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LoginMasterKey _key;

        public LoginMasterKeyCheckingMiddleware(RequestDelegate next, IOptions<LoginMasterKey> configuration)
        {
            _next = next;
            _key = configuration.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/session/login", StringComparison.OrdinalIgnoreCase))
            {
                // Validate the x-master-key header
                if (!context.Request.Headers.TryGetValue("x-master-key", out var key) || key != _key.XMasterKey)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid or missing x-master-key.");
                    return;
                }
            }
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }
}
