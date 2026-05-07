using System.Linq;
using HelperZaOptimalnuKupnju.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelperZaOptimalnuKupnju.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.OrderItems
                .Include(i => i.Product)
                .Include(i => i.Order)
                .ToList());
        }

        public IActionResult Details(int id)
        {
            var item = _context.OrderItems
                .Include(i => i.Product)
                .Include(i => i.Order)
                .FirstOrDefault(i => i.Id == id);

            if (item == null) return NotFound();
            return View(item);
        }
    }
}
