public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token) || !IsTokenValid(token))
        {
            // Return 401 Unauthorized response
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("{\"error\":\"Unauthorized\"}");
            return;
        }

        await _next(context); // Proceed to the next middleware
    }

    private bool IsTokenValid(string token)
    {
        // Add your token validation logic here
        return token == "valid-token"; // Example validation
    }
}
