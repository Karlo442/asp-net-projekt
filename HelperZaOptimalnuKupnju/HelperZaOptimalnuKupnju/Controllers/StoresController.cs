using System.Linq;
using HelperZaOptimalnuKupnju.MockData;
using Microsoft.AspNetCore.Mvc;

namespace HelperZaOptimalnuKupnju.Controllers
{
    public class StoresController : Controller
    {
        public IActionResult Index()
        {
            return View(MockRepository.Stores);
        }

        public IActionResult Details(int id)
        {
            var store = MockRepository.GetStore(id);
            if (store == null) return NotFound();
            return View(store);
        }
    }
}
