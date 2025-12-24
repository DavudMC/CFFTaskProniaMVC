using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplicationPronia.Contexts;
using WebApplicationPronia.Entities;

namespace WebApplicationPronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(AppDBContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(x=>x.Category).ToListAsync();
            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> Create() 
        {
            await SendCategoriesToView();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid) 
            {
                await SendCategoriesToView();
                return View(product);
            }
            var isexistCategory = await _context.Categories.AnyAsync(x=>x.Id == product.CategoryId);
            if(!isexistCategory)
            {
                await SendCategoriesToView();
                ModelState.AddModelError("CategoryId", "Bele bir idli category movcud deyil!");
                return View(product);
            }
            await _context.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(int id) 
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) 
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) 
            {
                return NotFound();
            }
            await SendCategoriesToView();
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Product product) 
        {
            if (!ModelState.IsValid) 
            {
                await SendCategoriesToView();
                return View(product);
            }
            var isexistproduct = await _context.Products.FindAsync(product.Id);
            if(isexistproduct is null)
            {
                return BadRequest();
            }
            var isexistCategory = await _context.Categories.AnyAsync(x => x.Id == product.CategoryId);
            if (!isexistCategory)
            {
                await SendCategoriesToView();
                ModelState.AddModelError("CategoryId", "Bele bir idli category movcud deyil!");
                return View(product);
            }
            isexistproduct.Name = product.Name;
            isexistproduct.Description = product.Description;
            isexistproduct.Price = product.Price;
            isexistproduct.ImagePath = product.ImagePath;
            isexistproduct.CategoryId = product.CategoryId;
            _context.Products.Update(isexistproduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task SendCategoriesToView()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
        }
    }
}
