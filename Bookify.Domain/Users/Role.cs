namespace Bookify.Domain.Users;
public sealed class Role
{
    public static readonly Role Registered = new(1, "Registered"); // It will allow me manage my role from domain layer. and seed them using ef core.

    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; init; }
    
    public string Name { get; init; } = string.Empty;


    // navigation property that is going to point to the users entity.

    //public ICollection<User> Users { get; init; } = new List<User>();
    public ICollection<User> Users { get; init; } = [];

    // approach that we are using here doesn't strictly follow the Domain driven design principal. However this is more practicle way to manage role


    public ICollection<Permission> Permissions { get; set; } = [];
}

// we are not using the entity base class here because. We don't wanna use the GUID as the primary key.
