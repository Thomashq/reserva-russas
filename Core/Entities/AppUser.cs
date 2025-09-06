using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using RR.Core.Entities.Base;


namespace RR.Core.Entities
{
    public class AppUser : IdentityUser<int>, IBaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

    // Campos opcionais do domínio (adicione conforme precisar)
        public string? FullName { get; set; }
    }
}
