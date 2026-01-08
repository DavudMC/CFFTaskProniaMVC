using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplicationPronia.Abstractions;
using WebApplicationPronia.Contexts;

namespace WebApplicationPronia.Services
{
    public class BasketService(AppDBContext _context, IHttpContextAccessor _accessor) : IBasketService
    {
        public async Task<List<BasketItem>> GetBasketItemsAsync()
        {
            string userId = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var isexistUser = await _context.Users.AnyAsync(x => x.Id == userId);
            if (!isexistUser)
            {
                return [];
            }
            var basketItems = await _context.BasketItems.Include(x=>x.Product).Where(x=>x.AppUserId == userId).ToListAsync();
            return basketItems;
        }
    }
}
