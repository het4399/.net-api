public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Proceed to the next middleware
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Unhandled Exception: {ex.Message}");

            // Return consistent error response
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Internal server error.\"}");
        }
    }
}
