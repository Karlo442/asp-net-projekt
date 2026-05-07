using System.Linq;
using HelperZaOptimalnuKupnju.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelperZaOptimalnuKupnju.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Users.Include(u => u.Orders).ToList());
        }

        public IActionResult Details(int id)
        {
            var user = _context.Users
                .Include(u => u.Orders)
                .FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();
            return View(user);
        }
    }
}
