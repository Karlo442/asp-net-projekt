using System.Linq;
using HelperZaOptimalnuKupnju.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelperZaOptimalnuKupnju.Controllers
{
    [Route("proizv")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("lista")]
        public IActionResult Index()
        {
            return View(_context.Products.Include(p => p.Stores).ToList());
        }

        [Route("{id:int}")]
        [Route("detalji/{id:int}")]
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Stores)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();
            return View(product);
        }
    }
}
