using Microsoft.AspNetCore.Identity;

namespace ECommerce.IdentityService.API.Domain.Entities;

public sealed class Role : IdentityRole<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
