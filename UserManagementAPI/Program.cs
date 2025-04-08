using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        // 1. Error-handling middleware
        app.UseMiddleware<ErrorHandlingMiddleware>();

        // 2. Authentication middleware
        app.UseMiddleware<AuthenticationMiddleware>();

        // 3. Logging middleware
        app.UseMiddleware<LoggingMiddleware>();


        app.Run();
    }
}
public class User
{
    public int Id { get; set; }

    [Required]  // Ensures Name is always present
    [StringLength(50, MinimumLength = 2)]  // Sets Name length limits
    public string Name { get; set; }

    [Required]
    [EmailAddress]  // Validates proper email format
    public string Email { get; set; }

    [Required]
    [RegularExpression("Admin|Manager|Developer", ErrorMessage = "Role must be Admin, Manager, or Developer.")]
    public string Role { get; set; }

}


[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // Simulated in-memory database
    private static List<User> users = new List<User>();

    // GET: Retrieve all users
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        return Ok(users);
    }

    // GET: Retrieve a specific user by ID[HttpGet("{id}")]
    [HttpGet("{id}")]
    public ActionResult<User> GetUserById(int id)
    {
        try
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    // POST: Add a new user
    [HttpPost]
    public ActionResult AddUser([FromBody] User newUser)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.SelectMany(ms => ms.Value.Errors)
                                   .Select(e => e.ErrorMessage)
                                   .ToList();

            Console.WriteLine("Validation Errors:");
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }

            return BadRequest(ModelState);
        }


        newUser.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1; // Auto-generate ID
        users.Add(newUser);
        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
    }

    // PUT: Update an existing user's details
    [HttpPut("{id}")]
    public ActionResult UpdateUser(int id, [FromBody] User updatedUser)
    {
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }
        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        user.Role = updatedUser.Role;
        return NoContent();
    }

    // DELETE: Remove a user by ID
    [HttpDelete("{id}")]
    public ActionResult DeleteUser(int id)
    {
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }
        users.Remove(user);
        return NoContent();
    }
    [HttpGet("test")]
    public ActionResult<string> Test()
    {
        return "API is working!";
    }

}
