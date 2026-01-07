using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }
}
