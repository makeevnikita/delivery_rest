using Microsoft.AspNetCore.Mvc;
using delivery.Models;
using Microsoft.EntityFrameworkCore;


namespace delivery.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private ILogger<ProductController> _logger;

    private readonly UserContext _userContext;

    public UserController(ILogger<ProductController> logger, UserContext userContext)
    {
        _logger = logger;
        _userContext = userContext;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] UserDto user)
    {   
        try
        {
            if (await _userContext.Users.AnyAsync(w => w.Login == user.Login) == true)
            {
                return new JsonResult(new { reesult = "Exists", description = "the user already exists"});
            }

            HashHelper hash = new HashHelper();

            User newUser = new User {
                Login = user.Login,
                Password = hash.Hash(user.Password),
                GroupId = 1
            };

            await _userContext.Users.AddAsync(newUser);  
            await _userContext.SaveChangesAsync();  

            return Created(Url.RouteUrl("GetUserById", new {id = newUser.Id}), new { result = "success" });
        }
        catch
        {
            string message = $"failed to create new user";
            _logger.LogError(message);

            return NotFound(new { result = "Error", description = message } );
        }
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        try
        {
            var user = await _userContext.Users.FirstOrDefaultAsync();

            if (user == null)
            {
                string message = $"User with id = {id} not found";
                _logger.LogError(message);

                return NotFound(new { result = "NotFound", description = message });
            }
            
            return new JsonResult(user);
        }
        catch (Exception ex)
        {
            string message = $"an error occurred while searching for a user with id = {id}";

            _logger.LogError(message + '\n' + ex.Message + '\n' + ex.StackTrace);
            return new JsonResult(new { 
                    result = "error", 
                    description = message 
                }
            ); 
        }
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get()
    {
        try
        {
            var users = await _userContext.Users.ToListAsync();

            return new JsonResult(users);
        }
        catch (Exception ex)
        {
            string message = $"an error occurred while searching for groups";

            _logger.LogError(message + '\n' + ex.Message + '\n' + ex.StackTrace);
            return new JsonResult(new { 
                    result = "error", 
                    description = message 
                }
            );
        }
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove([FromQuery] int id)
    {
        User user = new User { Id = id };

        _userContext.Users.Attach(user);
        _userContext.Users.Remove(user);
    
        try
        {
            await _userContext.SaveChangesAsync();

            return new JsonResult(new { result = "success" });
        }
        catch(Exception ex)
        {
            string message = $"An error occurred during the deletion of user id = {id} or user does not exist";

            _logger.LogError(message + '\n' + ex.Message + '\n' + ex.StackTrace);
            return new JsonResult(new { 
                    result = "error", 
                    description = message 
                }
            ); 
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto userDto)
    {
        User user = new User { Id = userDto.Id };

        if (userDto.Login != null)
        {
            user.Login = userDto.Login;
        }

        if (userDto.Password != null)
        {
            user.Password = userDto.Password;
        }

        if (userDto.GroupId.HasValue)
        {
            user.GroupId = userDto.GroupId.Value;
        }

        _userContext.Users.Attach(user);
        _userContext.Users.Update(user);

        try
        {
            await _userContext.SaveChangesAsync();

            return new JsonResult(new { result = "Success", description = $"User with id = {userDto.Id} succefully updated"});
        }
        catch(Exception ex)
        {
            string message = $"An error occurred during the updating of the user id = {userDto.Id} or the user does not exist";

            _logger.LogError(message + '\n' + ex.Message + '\n' + ex.StackTrace);
            return new JsonResult(new { 
                    result = "error", 
                    description = message 
                }
            ); 
        }
    }
}

public class UserDto
{
    public string Login { get; set; }

    public string Password { get; set; }
}

public class UpdateUserDto
{
    public int Id { get; set; }

    public string Login { get; set; }

    public string Password { get; set; }

    public int? GroupId { get; set; }
}