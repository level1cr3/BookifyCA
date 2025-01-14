namespace Bookify.Domain.Users;
public class RolePermission
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}


// we are doing this to seed the value from migration. you could manage this directly from the database.
// we are create many to many relationship between 'role and permission'.