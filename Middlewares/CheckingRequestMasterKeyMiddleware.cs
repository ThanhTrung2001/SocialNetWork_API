namespace EnVietSocialNetWorkAPI.Middlewares
{
    public class CheckingRequestMasterKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public CheckingRequestMasterKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/session/login", StringComparison.OrdinalIgnoreCase))
            {
                // Validate the x-master-key header
                if (!context.Request.Headers.TryGetValue("x-master-key", out var key) || key != _configuration["X-Master-Key"])
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
