using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplicationPronia.Contexts;
using WebApplicationPronia.Entities;
using WebApplicationPronia.ViewModels;

namespace WebApplicationPronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(AppDBContext _context,IWebHostEnvironment _environment) : Controller
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
        public async Task<IActionResult> Create(ProductCreateVM productvm)
        {
            if (!ModelState.IsValid) 
            {
                await SendCategoriesToView();
                return View(productvm);
            }
            if(productvm.CategoryId == null)
            {
                await SendCategoriesToView();
                ModelState.AddModelError("CategoryId", "Categoriya null ola bilmez!");
                return View(productvm);
            }
            var isexistCategory = await _context.Categories.AnyAsync(x=>x.Id == productvm.CategoryId);
            if(!isexistCategory)
            {
                await SendCategoriesToView();
                ModelState.AddModelError("CategoryId", "Bele bir idli category movcud deyil!");
                return View(productvm);
            }
            if (!productvm.MainImage.ContentType.Contains("image"))
            {
                ModelState.AddModelError("MainImage", "Yalniz sekil formatinda data daxil edile biler!");
                return View(productvm);
            }
            if (productvm.MainImage.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("MainImage", "Image-in size-i 2 mb-dan cox olmamalidir!");
                return View(productvm);
            }
            if (!productvm.HoverImage.ContentType.Contains("image"))
            {
                ModelState.AddModelError("HoverImage", "Yalniz sekil formatinda data daxil edile biler!");
                return View(productvm);
            }
            if (productvm.HoverImage.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("HoverImage", "Image-in size-i 2 mb-dan cox olmamalidir!");
                return View(productvm);
            }
            string uniqueMainImageName = Guid.NewGuid().ToString() + productvm.MainImage.FileName;
            string mainimagePath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images", uniqueMainImageName);
            using FileStream mainstream = new FileStream(mainimagePath,FileMode.Create);
            await productvm.MainImage.CopyToAsync(mainstream);
            string uniqueHoverImageName = Guid.NewGuid().ToString() + productvm.HoverImage.FileName;
            string hoverimagePath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images", uniqueHoverImageName);
            using FileStream hoverstream = new FileStream(hoverimagePath, FileMode.Create);
            await productvm.HoverImage.CopyToAsync(hoverstream);
            Product product = new() 
            {
                Name = productvm.Name,
                CategoryId = productvm.CategoryId,
                Description = productvm.Description,
                Price = productvm.Price,
                Rating = productvm.Rating,
                MainImagePath = uniqueMainImageName,
                HoverImagePath = uniqueHoverImageName
            };

            await _context.Products.AddAsync(product);
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
            string folderaPath = Path.Combine(_environment.WebRootPath,"assets","images","website-images");
            string mainimagePath = Path.Combine(folderaPath, product.MainImagePath);
            string hoverimagePath = Path.Combine(folderaPath, product.HoverImagePath);
            if(System.IO.File.Exists(mainimagePath))
            {
                System.IO.File.Delete(mainimagePath);
            }
            
            if(System.IO.File.Exists(hoverimagePath))
            {
                System.IO.File.Delete(hoverimagePath);
            }
            
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
        public async Task<IActionResult> Update(ProductUpdateVM productvm) 
        {
            if (!ModelState.IsValid) 
            {
                await SendCategoriesToView();
                return View(productvm);
            }
            var isexistproduct = await _context.Products.FindAsync(productvm.Id);
            if(isexistproduct is null)
            {
                return BadRequest();
            }
            var isexistCategory = await _context.Categories.AnyAsync(x => x.Id == productvm.CategoryId);
            if (!isexistCategory)
            {
                await SendCategoriesToView();
                ModelState.AddModelError("CategoryId", "Bele bir idli category movcud deyil!");
                return View(productvm);
            }
            if (!productvm.MainImage.ContentType.Contains("image"))
            {
                ModelState.AddModelError("MainImage", "Yalniz sekil formatinda data daxil edile biler!");
                return View(productvm);
            }
            if (productvm.MainImage.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("MainImage", "Image-in size-i 2 mb-dan cox olmamalidir!");
                return View(productvm);
            }
            if (!productvm.HoverImage.ContentType.Contains("image"))
            {
                ModelState.AddModelError("HoverImage", "Yalniz sekil formatinda data daxil edile biler!");
                return View(productvm);
            }
            if (productvm.HoverImage.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("HoverImage", "Image-in size-i 2 mb-dan cox olmamalidir!");
                return View(productvm);
            }
            string uniqueMainImageName = Guid.NewGuid().ToString() + productvm.MainImage.FileName;
            string mainimagePath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images", uniqueMainImageName);
            using FileStream mainstream = new FileStream(mainimagePath, FileMode.Create);
            await productvm.MainImage.CopyToAsync(mainstream);
            string uniqueHoverImageName = Guid.NewGuid().ToString() + productvm.HoverImage.FileName;
            string hoverimagePath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images", uniqueHoverImageName);
            using FileStream hoverstream = new FileStream(hoverimagePath, FileMode.Create);
            await productvm.HoverImage.CopyToAsync(hoverstream);
            isexistproduct.Name = productvm.Name;
            isexistproduct.Description = productvm.Description;
            isexistproduct.Price = productvm.Price;
            isexistproduct.MainImagePath = uniqueMainImageName;
            isexistproduct.HoverImagePath = uniqueHoverImageName;
            isexistproduct.CategoryId = productvm.CategoryId;
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
