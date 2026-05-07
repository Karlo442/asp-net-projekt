using System.Linq;
using HelperZaOptimalnuKupnju.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelperZaOptimalnuKupnju.Controllers
{
    [Route("narudz")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("sve")]
        public IActionResult Index()
        {
            return View(_context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .ToList());
        }

        [Route("{id:int}")]
        [Route("prikazi/{id:int}")]
        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }
    }
}
