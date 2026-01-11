using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplicationPronia.Abstractions;
using WebApplicationPronia.Contexts;
using WebApplicationPronia.Entities;

namespace WebApplicationPronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class InfoCardController(AppDBContext _context,ICloudinaryService _cloudinaryService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var infocards = await _context.InfoCards.ToListAsync();
            return View(infocards);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(InfoCard infoCard) 
        {
            if (!ModelState.IsValid) 
            {
                return View();
            }
            //===>//
            //infoCard.ImagePath = await _cloudinaryService.FileUploadAsync(infoCard.Image); Must be fixed
            //<==//

            await _context.InfoCards.AddAsync(infoCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var infocard = await _context.InfoCards.FindAsync(id);
            if(infocard is null)
                return NotFound();
            _context.InfoCards.Remove(infocard);
            await _context.SaveChangesAsync();
            //await _cloudinaryService.FileDeleteAsync(infocard.ImagePath); must be fixed;
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var infocard = await _context.InfoCards.FindAsync(id);
            if (infocard is not { })
                return NotFound();
            return View(infocard);
        }
        [HttpPost]
        public async Task<IActionResult> Update(InfoCard infoCard)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existinfoCard = await _context.InfoCards.FindAsync(infoCard.Id);
            if (existinfoCard is null)
                return BadRequest();
            existinfoCard.Title = infoCard.Title;
            existinfoCard.Description = infoCard.Description;
            existinfoCard.ImagePath = infoCard.ImagePath;
            _context.InfoCards.Update(existinfoCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
