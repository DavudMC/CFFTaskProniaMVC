using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationPronia.Contexts;

namespace WebApplicationPronia.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDBContext _context;
        public HomeController(AppDBContext context)
        {
            _context=context;
        }
        [Authorize]
        public IActionResult Index()
        {
            var infocards = _context.InfoCards.ToList();
            ViewBag.InfoCards = infocards;
            return View();
        }
    }
}
