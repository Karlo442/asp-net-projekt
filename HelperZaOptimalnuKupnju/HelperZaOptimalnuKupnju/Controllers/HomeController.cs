using HelperZaOptimalnuKupnju.MockData;
using HelperZaOptimalnuKupnju.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HelperZaOptimalnuKupnju.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.EntityCounts = new Dictionary<string, int>
            {
                ["Users"] = MockRepository.Users.Count,
                ["Stores"] = MockRepository.Stores.Count,
                ["Products"] = MockRepository.Products.Count,
                ["Orders"] = MockRepository.Orders.Count,
                ["OrderItems"] = MockRepository.OrderItems.Count
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
