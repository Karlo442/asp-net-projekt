using HelperZaOptimalnuKupnju.Data;
using HelperZaOptimalnuKupnju.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HelperZaOptimalnuKupnju.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.EntityCounts = new Dictionary<string, int>
            {
                ["Users"] = _context.Users.Count(),
                ["Stores"] = _context.Stores.Count(),
                ["Products"] = _context.Products.Count(),
                ["Orders"] = _context.Orders.Count(),
                ["OrderItems"] = _context.OrderItems.Count()
            };
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
