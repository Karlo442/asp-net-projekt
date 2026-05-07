using System.IO;
using HelperZaOptimalnuKupnju.Data;
using HelperZaOptimalnuKupnju.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace HelperZaOptimalnuKupnju.Controllers
{
    [Route("baze")]
    public class DatabasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DatabasesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            var model = new DatabaseViewModel
            {
                Users = _context.Users.Count(),
                Products = _context.Products.Count(),
                Orders = _context.Orders.Count(),
                Stores = _context.Stores.Count(),
                OrderItems = _context.OrderItems.Count(),
                DbPath = Path.Combine(_env.ContentRootPath, "HelperZaOptimalnuKupnju.db")
            };

            return View(model);
        }

        [Route("download")]
        public IActionResult Download()
        {
            var path = Path.Combine(_env.ContentRootPath, "HelperZaOptimalnuKupnju.db");
            if (!System.IO.File.Exists(path)) return NotFound();
            return PhysicalFile(path, "application/octet-stream", "HelperZaOptimalnuKupnju.db");
        }
    }
}
