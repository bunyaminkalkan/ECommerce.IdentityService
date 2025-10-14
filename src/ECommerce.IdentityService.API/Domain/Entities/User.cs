using Microsoft.AspNetCore.Identity;

namespace ECommerce.IdentityService.API.Domain.Entities;

public sealed class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Role> UserRoles { get; set; } = new List<Role>();
}
