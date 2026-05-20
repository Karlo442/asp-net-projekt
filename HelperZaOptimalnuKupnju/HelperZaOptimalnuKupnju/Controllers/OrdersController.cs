using System;
using System.Linq;
using HelperZaOptimalnuKupnju.Data;
using HelperZaOptimalnuKupnju.Models;
using HelperZaOptimalnuKupnju.Models.ViewModels;
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
        public IActionResult Index(string search = "")
        {
            var query = _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o => 
                    o.Buyer.FirstName.Contains(search) || 
                    o.Buyer.LastName.Contains(search) ||
                    o.Status.ToString().Contains(search));
            }

            var orders = query.ToList();
            ViewData["SearchTerm"] = search;
            return View(orders);
        }

        [Route("pretraga")]
        [HttpGet]
        public IActionResult Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new { data = new object[] { } });
            }

            var results = _context.Orders
                .Include(o => o.Buyer)
                .Where(o => 
                    o.Buyer.FirstName.Contains(term) || 
                    o.Buyer.LastName.Contains(term) ||
                    o.Status.ToString().Contains(term))
                .Select(o => new 
                { 
                    id = o.Id, 
                    buyerName = o.Buyer.FirstName + " " + o.Buyer.LastName,
                    status = o.Status.ToString(),
                    totalAmount = o.TotalAmount,
                    createdAt = o.CreatedAt.ToString("yyyy-MM-dd")
                })
                .Take(20)
                .ToList();

            return Json(new { data = results });
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

        [Route("novo")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Buyers"] = _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.FirstName)
                .ToList();
            
            return View(new OrderCreateViewModel());
        }

        [Route("novo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrderCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var buyer = _context.Users.Find(model.BuyerId);
                    if (buyer == null)
                    {
                        ModelState.AddModelError("BuyerId", "Kupac nije pronađen.");
                        ViewData["Buyers"] = _context.Users.Where(u => u.IsActive).OrderBy(u => u.FirstName).ToList();
                        return View(model);
                    }

                    var order = new Order
                    {
                        BuyerId = model.BuyerId,
                        CreatedAt = DateTime.Now,
                        ExpectedDeliveryDateTime = model.ExpectedDeliveryDateTime,
                        Status = model.Status,
                        TotalAmount = 0m
                    };

                    _context.Orders.Add(order);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Details), new { id = order.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Greška pri spremi podataka: {ex.Message}");
                }
            }

            ViewData["Buyers"] = _context.Users.Where(u => u.IsActive).OrderBy(u => u.FirstName).ToList();
            return View(model);
        }

        [Route("uredi/{id:int}")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = _context.Orders.Include(o => o.Buyer).FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            var model = new OrderEditViewModel
            {
                Id = order.Id,
                BuyerId = order.BuyerId,
                ExpectedDeliveryDateTime = order.ExpectedDeliveryDateTime,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount
            };

            ViewData["Buyers"] = _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.FirstName)
                .ToList();

            return View(model);
        }

        [Route("uredi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, OrderEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var order = _context.Orders.Find(id);
                    if (order == null) return NotFound();

                    var buyer = _context.Users.Find(model.BuyerId);
                    if (buyer == null)
                    {
                        ModelState.AddModelError("BuyerId", "Kupac nije pronađen.");
                        ViewData["Buyers"] = _context.Users.Where(u => u.IsActive).OrderBy(u => u.FirstName).ToList();
                        return View(model);
                    }

                    order.BuyerId = model.BuyerId;
                    order.ExpectedDeliveryDateTime = model.ExpectedDeliveryDateTime;
                    order.Status = model.Status;

                    _context.Orders.Update(order);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Details), new { id = order.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Greška pri ažuriranju: {ex.Message}");
                }
            }

            ViewData["Buyers"] = _context.Users.Where(u => u.IsActive).OrderBy(u => u.FirstName).ToList();
            return View(model);
        }

        [Route("obrisi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            try
            {
                if (order.Items != null && order.Items.Any())
                {
                    _context.OrderItems.RemoveRange(order.Items);
                }
                
                _context.Orders.Remove(order);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return BadRequest($"Ne mogu obrisati narudžbu: {ex.Message}");
            }
        }

        [Route("autocomplete-buyers")]
        [HttpGet]
        public IActionResult AutocompleteBuyers(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new { data = new object[] { } });
            }

            var results = _context.Users
                .Where(u => u.IsActive && (u.FirstName.Contains(term) || u.LastName.Contains(term)))
                .Select(u => new { id = u.Id, text = u.FirstName + " " + u.LastName })
                .Take(10)
                .ToList();

            return Json(new { data = results });
        }
    }
}
