using System.Linq;
using HelperZaOptimalnuKupnju.MockData;
using Microsoft.AspNetCore.Mvc;

namespace HelperZaOptimalnuKupnju.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View(MockRepository.Products);
        }

        public IActionResult Details(int id)
        {
            var product = MockRepository.GetProduct(id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}
