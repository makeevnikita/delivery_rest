using Microsoft.EntityFrameworkCore;



namespace delivery.Models;

public class Permission
{
    public int Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public List<GroupPermission> GroupPermissions { get; set; }
}

public class Group
{
    public int Id { get; set; }

    public string Name { get; set; }

    public List<GroupPermission> GroupPermissions { get; set; }
}

public class GroupPermission
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public Group Group { get; set; }

    public int PermissionId { get; set; }

    public Permission Permission { get; set; }
}

public class User
{
    public int Id { get; set; }

    public string Login { get; set; }

    public string Password { get; set; }

    public int GroupId { get; set; }

    public Group Group { get; set; }
}

public class UserContext : DbContext
{
    public DbSet<Permission> Permissions { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Group> Groups { get; set; }

    public DbSet<GroupPermission> GroupPermissions { get; set; }

    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {

    }   
}
