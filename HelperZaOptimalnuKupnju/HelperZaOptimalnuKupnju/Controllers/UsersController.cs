using System.Linq;
using HelperZaOptimalnuKupnju.MockData;
using Microsoft.AspNetCore.Mvc;

namespace HelperZaOptimalnuKupnju.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View(MockRepository.Users);
        }

        public IActionResult Details(int id)
        {
            var user = MockRepository.GetUser(id);
            if (user == null) return NotFound();
            return View(user);
        }
    }
}
