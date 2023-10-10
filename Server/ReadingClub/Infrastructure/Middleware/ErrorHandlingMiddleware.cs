using System.Text.Json;

namespace ReadingClub.Infrastructure.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var errorResponse = new
                {
                    status = false,
                    message = "An unexpected error occurred during processing."
                };

                var jsonErrorResponse = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(jsonErrorResponse);
            }
        }
    }
}
