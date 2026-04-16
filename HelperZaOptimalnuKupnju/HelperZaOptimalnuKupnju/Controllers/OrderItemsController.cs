using System.Linq;
using HelperZaOptimalnuKupnju.MockData;
using Microsoft.AspNetCore.Mvc;

namespace HelperZaOptimalnuKupnju.Controllers
{
    public class OrderItemsController : Controller
    {
        public IActionResult Index()
        {
            return View(MockRepository.OrderItems);
        }

        public IActionResult Details(int id)
        {
            var item = MockRepository.GetOrderItem(id);
            if (item == null) return NotFound();
            return View(item);
        }
    }
}
