using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplicationPronia.Contexts;
using WebApplicationPronia.Entities;
using WebApplicationPronia.Helpers;
using WebApplicationPronia.ViewModels.ProductViewModels;

namespace WebApplicationPronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(AppDBContext _context,IWebHostEnvironment _environment) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Select(product => new ProductGetVM() 
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryName = product.Category.Name,
                HoverImagePath = product.HoverImagePath,
                MainImagePath = product.MainImagePath,
                Price = product.Price,
                Rating = product.Rating

            }).ToListAsync();


            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> Create() 
        {
            await SendItemsToView();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM productvm)
        {
            if (!ModelState.IsValid) 
            {
                await SendItemsToView();
                return View(productvm);
            }
            var isexistCategory = await _context.Categories.AnyAsync(x=>x.Id == productvm.CategoryId);
            if(!isexistCategory)
            {
                await SendItemsToView();
                ModelState.AddModelError("CategoryId", "Bele bir idli category movcud deyil!");
                return View(productvm);
            }
            foreach (var tagId in productvm.TagIds) 
            {
                var isexistTagId = await _context.Tags.AnyAsync(y => y.Id == tagId);
                if (!isexistTagId) 
                {
                    await SendItemsToView();
                    ModelState.AddModelError("TagIds", "Bele bir tag movcud deyil");
                    return View(productvm);
                }
            }
            if (!productvm.MainImage.CheckType())
            {
                ModelState.AddModelError("MainImage", "Yalniz sekil formatinda data daxil edile biler!");
                return View(productvm);
            }
            if (!productvm.MainImage.CheckSize(2))
            {
                ModelState.AddModelError("MainImage", "Image-in size-i 2 mb-dan cox olmamalidir!");
                return View(productvm);
            }
            if (!productvm.HoverImage.CheckType())
            {
                ModelState.AddModelError("HoverImage", "Yalniz sekil formatinda data daxil edile biler!");
                return View(productvm);
            }
            if (!productvm.HoverImage.CheckSize(2))
            {
                ModelState.AddModelError("HoverImage", "Image-in size-i 2 mb-dan cox olmamalidir!");
                return View(productvm);
            }
            foreach(var image in productvm.Images)
            {
                if (!image.CheckType())
                {
                    ModelState.AddModelError("HoverImage", "Yalniz sekil formatinda data daxil edile biler!");
                    return View(productvm);
                }
                if (!image.CheckSize(2))
                {
                    ModelState.AddModelError("HoverImage", "Image-in size-i 2 mb-dan cox olmamalidir!");
                    return View(productvm);
                }
            }
            string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");
            string uniqueMainImageName = await productvm.MainImage.SaveFileAsync(folderPath);
            string uniqueHoverImageName = await productvm.HoverImage.SaveFileAsync(folderPath);
            //string uniqueMainImageName = Guid.NewGuid().ToString() + productvm.MainImage.FileName;
            //string mainimagePath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images", uniqueMainImageName);
            //using FileStream mainstream = new FileStream(mainimagePath,FileMode.Create);
            //await productvm.MainImage.CopyToAsync(mainstream);
            //string uniqueHoverImageName = Guid.NewGuid().ToString() + productvm.HoverImage.FileName;
            //string hoverimagePath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images", uniqueHoverImageName);
            //using FileStream hoverstream = new FileStream(hoverimagePath, FileMode.Create);
            //await productvm.HoverImage.CopyToAsync(hoverstream);
            Product product = new() 
            {
                Name = productvm.Name,
                CategoryId = productvm.CategoryId,
                Description = productvm.Description,
                Price = productvm.Price,
                Rating = productvm.Rating,
                MainImagePath = uniqueMainImageName,
                HoverImagePath = uniqueHoverImageName,
                ProductTags = [],
                ProductImages = []
            };
            foreach (var image in productvm.Images) 
            {
                string uniquefilePath = await image.SaveFileAsync(folderPath);
                ProductImage productImage = new()
                {
                    ImagePath = uniquefilePath,
                    Product = product,
                };
                product.ProductImages.Add(productImage);
            }
            
            foreach(var tagId in productvm.TagIds)
            {
                ProductTag productTag = new ProductTag()
                {
                    TagId = tagId,
                    Product = product
                };
                product.ProductTags.Add(productTag);
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(int id) 
        {
            var product = await _context.Products.Include(images=>images.ProductImages).FirstOrDefaultAsync(x=>x.Id == id);
            if (product == null) 
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            string folderPath = Path.Combine(_environment.WebRootPath,"assets","images","website-images");
            string mainimagePath = Path.Combine(folderPath, product.MainImagePath);
            string hoverimagePath = Path.Combine(folderPath, product.HoverImagePath);
            
            ExtensionMethods.DeleteFile(mainimagePath);
            
            ExtensionMethods.DeleteFile(hoverimagePath);
            foreach(var productImage in product.ProductImages)
            {
                string imagePath = Path.Combine(folderPath, productImage.ImagePath);
                ExtensionMethods.DeleteFile(imagePath);
            }
            
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _context.Products.Include(x=> x.ProductTags).Include(x=>x.ProductImages).FirstOrDefaultAsync(x=>x.Id == id);
            if (product == null) 
            {
                return NotFound();
            }
            await SendItemsToView();
            ProductUpdateVM vm = new ProductUpdateVM() 
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Price = product.Price,
                MainImagePath = product.MainImagePath,
                HoverImagePath = product.HoverImagePath,
                TagIds = product.ProductTags.Select(t => t.TagId).ToList(),
                AdditionalImagePaths = product.ProductImages.Select(testc=> testc.ImagePath).ToList(),
                AdditionalImageIds = product.ProductImages.Select(x=>x.Id).ToList()
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductUpdateVM productvm) 
        {
            if (!ModelState.IsValid) 
            {
                await SendItemsToView();
                return View(productvm);
            }
            var isexistproduct = await _context.Products.Include(x=>x.ProductTags).Include(x=>x.ProductImages).FirstOrDefaultAsync(x=>x.Id == productvm.Id);
            if(isexistproduct is null)
            {
                return BadRequest();
            }
            var isexistCategory = await _context.Categories.AnyAsync(x => x.Id == productvm.CategoryId);
            if (!isexistCategory)
            {
                await SendItemsToView();
                ModelState.AddModelError("CategoryId", "Bele bir idli category movcud deyil!");
                return View(productvm);
            }
            foreach (var tagId in productvm.TagIds)
            {
                var isexistTagId = await _context.Tags.AnyAsync(y => y.Id == tagId);
                if (!isexistTagId)
                {
                    await SendItemsToView();
                    ModelState.AddModelError("TagIds", "Bele bir tag movcud deyil");
                    return View(productvm);
                }
            }
            if (!productvm.MainImage?.CheckType() ?? false)
            {
                ModelState.AddModelError("MainImage", "Yalniz sekil formatinda data daxil edile biler!");
                return View(productvm);
            }
            if (!productvm.MainImage?.CheckSize(2) ?? false)
            {
                ModelState.AddModelError("MainImage", "Image-in size-i 2 mb-dan cox olmamalidir!");
                return View(productvm);
            }
            if (!productvm.HoverImage?.CheckType() ?? false)
            {
                ModelState.AddModelError("HoverImage", "Yalniz sekil formatinda data daxil edile biler!");
                return View(productvm);
            }
            if (!productvm.HoverImage?.CheckSize(2) ?? false)
            {
                ModelState.AddModelError("HoverImage", "Image-in size-i 2 mb-dan cox olmamalidir!");
                return View(productvm);
            }
            foreach (var image in productvm.Images ?? new())
            {
                if (!image.CheckType())
                {
                    ModelState.AddModelError("HoverImage", "Yalniz sekil formatinda data daxil edile biler!");
                    return View(productvm);
                }
                if (!image.CheckSize(2))
                {
                    ModelState.AddModelError("HoverImage", "Image-in size-i 2 mb-dan cox olmamalidir!");
                    return View(productvm);
                }
            }
            //string uniqueMainImageName = Guid.NewGuid().ToString() + productvm.MainImage.FileName;
            //string mainimagePath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images", uniqueMainImageName);
            //using FileStream mainstream = new FileStream(mainimagePath, FileMode.Create);
            //await productvm.MainImage.CopyToAsync(mainstream);
            //string uniqueHoverImageName = Guid.NewGuid().ToString() + productvm.HoverImage.FileName;
            //string hoverimagePath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images", uniqueHoverImageName);
            //using FileStream hoverstream = new FileStream(hoverimagePath, FileMode.Create);
            //await productvm.HoverImage.CopyToAsync(hoverstream);
            isexistproduct.Name = productvm.Name;
            isexistproduct.Description = productvm.Description;
            isexistproduct.Price = productvm.Price;
            isexistproduct.CategoryId = productvm.CategoryId;
            isexistproduct.ProductTags = [];
            foreach(var tagId in productvm.TagIds)
            {
                ProductTag productTag = new ProductTag()
                {
                    TagId = tagId,
                    ProductId = isexistproduct.Id
                };
                isexistproduct.ProductTags.Add(productTag);
            }
            

            string folderPath = Path.Combine(_environment.WebRootPath, "assets","images","website-images");
            
            if (productvm.MainImage is { })
            {
                string newMainImagePath = await productvm.MainImage.SaveFileAsync(folderPath);
                string existMainImagePath = Path.Combine(_environment.WebRootPath, isexistproduct.MainImagePath);
                ExtensionMethods.DeleteFile(existMainImagePath);
                isexistproduct.MainImagePath = newMainImagePath;
            }
            if (productvm.HoverImage is { })
            {
                string newHoverImagePath = await productvm.HoverImage.SaveFileAsync(folderPath);
                string existHoverImagePath = Path.Combine(_environment.WebRootPath, isexistproduct.HoverImagePath);
                ExtensionMethods.DeleteFile(existHoverImagePath);
                isexistproduct.HoverImagePath = newHoverImagePath;
            }
            var existImages = isexistproduct.ProductImages.ToList();
            foreach(var image in existImages)
            {
                var isexistImageId = productvm.AdditionalImageIds?.Any(x=>x == image.Id) ?? false;
                if(!isexistImageId)
                {
                    string deletedImagePath = Path.Combine(folderPath, image.ImagePath);
                    ExtensionMethods.DeleteFile(deletedImagePath);
                    isexistproduct.ProductImages.Remove(image);
                }
            }
            foreach (var image in productvm.Images ?? new())
            {
                string uniquefilePath = await image.SaveFileAsync(folderPath);
                ProductImage productImage = new()
                {
                    ImagePath = uniquefilePath,
                    ProductId = isexistproduct.Id,
                };
                isexistproduct.ProductImages.Add(productImage);
            }
            _context.Products.Update(isexistproduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task SendItemsToView()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            var tags = await _context.Tags.ToListAsync();
            ViewBag.Tags = tags;
        }
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.Products.Select(product => new ProductGetVM()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryName = product.Category.Name,
                HoverImagePath = product.HoverImagePath,
                MainImagePath = product.MainImagePath,
                Price = product.Price,
                Rating = product.Rating,
                TagNames = product.ProductTags.Select(x=> x.Tag.Name).ToList(),
                AdditionalImagePaths = product.ProductImages.Select(x=>x.ImagePath).ToList()
            }).FirstOrDefaultAsync(x => x.Id == id);

            if (product == null) 
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
