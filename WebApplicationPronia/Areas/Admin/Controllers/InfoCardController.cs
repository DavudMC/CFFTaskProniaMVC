using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplicationPronia.Contexts;
using WebApplicationPronia.Entities;

namespace WebApplicationPronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InfoCardController(AppDBContext _context) : Controller
    {
        public async Task<IActionResult> IndexAsync()
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
        public async Task<IActionResult> Create(InfoCard ınfoCard) 
        {
            if (!ModelState.IsValid) 
            {
                return View();
            }
            await _context.InfoCards.AddAsync(ınfoCard);
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
            return RedirectToAction(nameof(Index));
        }
    }
}
