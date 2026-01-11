using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplicationPronia.Abstractions;
using WebApplicationPronia.Contexts;

namespace WebApplicationPronia.Controllers
{
    public class BasketController(IBasketService _basketService,AppDBContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var basketItems = await _basketService.GetBasketItemsAsync();
            return View(basketItems);
        }
        public async Task<IActionResult> DecreaseBasketItemCount(int productId)
        {
            var existPoduct = await _context.Products.AnyAsync(x => x.Id == productId);
            if (existPoduct == false)
            {
                return NotFound();
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var isexistUser = await _context.Users.AnyAsync(u => u.Id == userId);
            if (isexistUser == false)
            {
                return BadRequest();
            }
            var basketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.ProductId == productId && x.AppUserId == userId);
            if (basketItem == null)
            {
                return NotFound();
            }
            if(basketItem.Count>1)
                basketItem.Count--;

            _context.BasketItems.Update(basketItem);
            await _context.SaveChangesAsync();
            //string? returnUrl = Request.Headers["Referer"];
            //if (!string.IsNullOrWhiteSpace(returnUrl))
            //{
            //    return Redirect(returnUrl);
            //}
            var basketItems = await _basketService.GetBasketItemsAsync();
            return PartialView("_BasketPartialView", basketItems);
        }
        public async Task<IActionResult> IncreaseBasketItemCount(int productId)
        {
            var existPoduct = await _context.Products.AnyAsync(x => x.Id == productId);
            if (existPoduct == false)
            {
                return NotFound();
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var isexistUser = await _context.Users.AnyAsync(u => u.Id == userId);
            if (isexistUser == false)
            {
                return BadRequest();
            }
            var basketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.ProductId == productId && x.AppUserId == userId);
            if (basketItem == null)
            {
                return NotFound();
            }
            
                basketItem.Count++;

            _context.BasketItems.Update(basketItem);
            await _context.SaveChangesAsync();
            //string? returnUrl = Request.Headers["Referer"];
            //if (!string.IsNullOrWhiteSpace(returnUrl))
            //{
            //    return Redirect(returnUrl);
            //}
            var basketItems = await _basketService.GetBasketItemsAsync();
            return PartialView("_BasketPartialView", basketItems);
        }
    }
}
