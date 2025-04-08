public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log HTTP method and request path
        Console.WriteLine($"HTTP Request: {context.Request.Method} {context.Request.Path}");

        await _next(context);

        // Log Response status code
        Console.WriteLine($"HTTP Response: {context.Response.StatusCode}");
    }
}
