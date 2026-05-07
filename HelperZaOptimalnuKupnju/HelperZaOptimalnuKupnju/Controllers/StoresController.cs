using System.Linq;
using HelperZaOptimalnuKupnju.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelperZaOptimalnuKupnju.Controllers
{
    [Route("trgov")]
    public class StoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("sve")]
        public IActionResult Index()
        {
            return View(_context.Stores.Include(s => s.Products).ToList());
        }

        [Route("{id:int}")]
        [Route("info/{id:int}")]
        public IActionResult Details(int id)
        {
            var store = _context.Stores
                .Include(s => s.Products)
                .FirstOrDefault(s => s.Id == id);

            if (store == null) return NotFound();
            return View(store);
        }
    }
}
