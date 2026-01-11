using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplicationPronia.Abstractions;
using WebApplicationPronia.Contexts;
using WebApplicationPronia.ViewModels.ProductViewModels;

namespace WebApplicationPronia.Controllers
{
    
    public class ShopController(AppDBContext _context,IEmailService _emailService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Test()
        {
            await _emailService.SendEmailAsync("davudfm-mpa101@code.edu.az", "MPA-101", "Email Service is done");
            return Ok("Ok");
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.Products.Select(x => new ProductGetVM() 
            {
                Id  = x.Id,
                Name = x.Name,
                Description = x.Description,
                AdditionalImagePaths = x.ProductImages.Select(x => x.ImagePath).ToList(),
                CategoryName = x.Category.Name,
                HoverImagePath = x.HoverImagePath,
                MainImagePath = x.MainImagePath,
                Price = x.Price,
                Rating = x.Rating,
                TagNames = x.ProductTags.Select(x=> x.Tag.Name).ToList()
            }).FirstOrDefaultAsync(x => x.Id == id);
            if(product is null)
            {
                return NotFound();
            }
            return View(product);
        }
        [Authorize]
        public async Task<IActionResult> AddToBasket(int productId)
        {
            var existPoduct = await _context.Products.AnyAsync(x=>x.Id == productId);
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
            var isexistbasketItem = await _context.BasketItems.FirstOrDefaultAsync(x=>x.ProductId == productId && x.AppUserId == userId);
            if(isexistbasketItem is { })
            {
                isexistbasketItem.Count++;
                _context.BasketItems.Update(isexistbasketItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                BasketItem basketItem = new BasketItem()
                {
                    ProductId = productId,
                    AppUserId = userId,
                    Count = 1
                };
                await _context.BasketItems.AddAsync(basketItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<IActionResult> RemoveFromBasket(int productId)
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
            _context.BasketItems.Remove(basketItem);
            await _context.SaveChangesAsync();
            string? returnUrl = Request.Headers["Referer"];
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");

        }
    }
}
