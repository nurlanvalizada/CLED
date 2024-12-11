using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CledAcademy.Core.Domain
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}