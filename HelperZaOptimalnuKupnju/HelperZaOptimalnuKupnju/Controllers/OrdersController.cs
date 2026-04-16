using System.Linq;
using HelperZaOptimalnuKupnju.MockData;
using Microsoft.AspNetCore.Mvc;

namespace HelperZaOptimalnuKupnju.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View(MockRepository.Orders);
        }

        public IActionResult Details(int id)
        {
            var order = MockRepository.GetOrder(id);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}
