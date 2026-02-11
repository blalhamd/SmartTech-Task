using Microsoft.AspNetCore.Identity;

namespace EduNexus.Domain.Entities.Identity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
