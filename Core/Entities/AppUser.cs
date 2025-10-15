using Microsoft.AspNetCore.Identity;
using RR.Core.Entities.Base;


namespace RR.Core.Entities
{
    public class AppUser : IdentityUser, IBaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

    // Campos opcionais do domínio (adicione conforme precisar)
        public Account Account { get; set; } = null!; // Relação 1:1 com Account
        public string? FullName { get; set; }
    }
}
