using UserManagement.Data;
using UserManagement.Models;
using Microsoft.AspNetCore.Mvc;


[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApiContext _context;

    public UserController(ApiContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Create(UserModel userModel)
    {
        if (userModel == null)
        {
            return BadRequest("User object is null");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingUser = _context.GetByUsername(userModel.Username);
        if (existingUser != null)
        {
            return BadRequest("Username already exists");
        }

        // Hash and salt the password here before saving
        // userModel.Password = HashPassword(userModel.Password); 
        _context.AddUser(userModel);

        return CreatedAtAction(nameof(GetByUsername), new { username = userModel.Username }, userModel);
    }

    [HttpPut("{username}")]
    public IActionResult Edit(string username, UserModel updatedUser)
    {
        var userInDb = _context.GetByUsername(username);
        if (userInDb == null)
        {
            return NotFound();
        }

        var existingUser = _context.GetByUsername(updatedUser.Username);
        if (existingUser != null && existingUser.Id != userInDb.Id)
        {
            return BadRequest("Username already exists");
        }

        // Only update the fields that can be safely modified by the client:
        userInDb.Username = updatedUser.Username;
        userInDb.Name = updatedUser.Name;
        userInDb.Email = updatedUser.Email;
        userInDb.Password = updatedUser.Password; // If updating the password, hash and salt here
        userInDb.Notes = updatedUser.Notes;

        _context.UpdateUser(userInDb);

        // Fetch the updated user from the database
        var newlyUpdatedUser = _context.GetByUsername(username);

        // Return a custom response without exposing the password
        return Ok(new
        {
            OldUser = new
            {
                userInDb.Id,
                userInDb.Username,
                userInDb.Name,
                userInDb.Email,
                userInDb.Notes
            },
            UpdatedUser = new
            {
                newlyUpdatedUser.Id,
                newlyUpdatedUser.Username,
                newlyUpdatedUser.Name,
                newlyUpdatedUser.Email,
                newlyUpdatedUser.Notes
            },
            Message = "User successfully updated"
        });
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _context.GetUsers();
        return Ok(users);
    }

    [HttpGet("{username}")]
    public IActionResult GetByUsername(string username)
    {
        var user = _context.GetByUsername(username);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpDelete("{username}")]
    public IActionResult DeleteByUsername(string username)
    {
        var user = _context.GetByUsername(username);
        if (user == null)
        {
            return NotFound();
        }

        _context.DeleteUserByUsername(username);

        return Ok(new
        {
            DeletedUser = new
            {
                user.Id,
                user.Username,
                user.Name,
                user.Email,
                // You might want to omit returning the Password.
                // user.Password, 
                user.Notes
            },
            Message = "User deleted"
        });
    }


    // Add a method to hash and salt passwords
    // private string HashPassword(string password) {...}
}
