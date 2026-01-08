using Microsoft.AspNetCore.Identity;

namespace WebApplicationPronia.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public ICollection<BasketItem> BasketItems { get; set; } = [];
    }
}
