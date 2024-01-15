using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Encodings.Web;
using delivery.Models;
using Microsoft.EntityFrameworkCore;



namespace delivery.Controllers;

[ApiController]
[Route("group")]
public class GroupController : ControllerBase
{
    private ILogger<GroupController> _logger;

    private readonly JsonSerializerOptions serializerOptions;

    private readonly IWebHostEnvironment _environment;

    private UserContext _userContext;
    
    public GroupController(UserContext userContext, ILogger<GroupController> logger)
    {
        _logger = logger;
        _userContext = userContext;
        
        serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] GroupDto groupDto)
    {
        Group group = new Group { Name = groupDto.Name};

        await _userContext.Groups.AddAsync(group);
        
        try
        {
            await _userContext.SaveChangesAsync();

            foreach(int permissionId in groupDto.Permissions)
            {
                GroupPermission groupPermission = new GroupPermission { GroupId = group.Id, PermissionId = permissionId };

                await _userContext.GroupPermissions.AddAsync(groupPermission);
            }

            await _userContext.SaveChangesAsync();

            return Created(Url.RouteUrl("GetGroupById", new {id = group.Id}), new { result = "success" });
        }
        catch (Exception ex)
        {
            string message = $"Failed to create a new group";
            _logger.LogError(message + "\n" + ex.Message + "\n" + ex.StackTrace);

            return new JsonResult(new { result = "Error", description = message });
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(GroupUpdateDto groupDto)
    {
        Group group = await _userContext.Groups.FirstOrDefaultAsync(w => w.Id == groupDto.Id);

        if (group == null)
        {
            string message = $"Group with id {groupDto.Id} not found";
            _logger.LogError(message);

            return NotFound(new { result = "NotFound", description = message });
        }

        group.Name = groupDto.Name;
        
        try
        {
            
            await _userContext.GroupPermissions.Where(w => groupDto.DeletedPermissionsId.Contains(w.Id))
                .ExecuteDeleteAsync();
            
            foreach(int id in groupDto.AddedPermissionsId)
            {
                GroupPermission groupPermission = new GroupPermission { GroupId = group.Id, PermissionId = id };

                await _userContext.GroupPermissions.AddAsync(groupPermission);
                await _userContext.SaveChangesAsync();
            }

            return new JsonResult(new { result = "Success", description = $"Group with id = {group.Id} succefully updated"});
        }
        catch
        {
            string message = $"Failed to update group with id {groupDto.Id}";
            _logger.LogError(message);

            return NotFound(new { result = "NotFound", description = message });
        }

    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        try
        {
            var group = await _userContext.Groups.Where(w => w.Id == id)
                    .Select(w => new {
                        Name = w.Name,
                        Permissions = w.GroupPermissions.Where(w => w.GroupId == id).Select(p => p.Permission.Code).ToList()
                    }).FirstOrDefaultAsync();

            if (group == null)
            {
                string message = $"Group with id = {id} not found";
                _logger.LogError(message);

                return NotFound(new { result = "NotFound", description = message });
            }

            return new JsonResult(group);
        }
        catch (Exception ex)
        {
            string message = $"an error occurred while searching for a group with id = {id}";

            _logger.LogError(message + '\n' + ex.Message + '\n' + ex.StackTrace);
            return new JsonResult(new { 
                    result = "error", 
                    description = message 
                }
            ); 
        }
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetAll()
    {   
        try
        {
            var groups = await _userContext.Groups.Select(w => new 
            { 
                Name = w.Name,
                Link = Url.RouteUrl("GetGroupById", new {id = w.Id})
            })
            .AsNoTracking().ToListAsync();

            return new JsonResult(groups);
        }
        catch(Exception ex)
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

    [HttpGet("remove")]
    public async Task<IActionResult> Remove([FromQuery] int id)
    {
        Group group = new Group { Id = id };

        _userContext.Groups.Attach(group);
        _userContext.Groups.Remove(group);

        try
        {
            await _userContext.SaveChangesAsync();

            return new JsonResult(new { result = "success" });
        }
        catch(Exception ex)
        {
            string message = $"An error occurred during the deletion of the group id = {id} or the group does not exist";

            _logger.LogError(message + '\n' + ex.Message + '\n' + ex.StackTrace);
            return new JsonResult(new { 
                    result = "error", 
                    description = message 
                }
            ); 
        }
    }
}

public class GroupDto
{
    public string Name { get; set; }

    public int[] Permissions { get; set; }
}

public class GroupUpdateDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int[]? DeletedPermissionsId { get; set; }

    public int[]? AddedPermissionsId { get; set; }
}