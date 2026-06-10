using System;
using System.Linq;
using HelperZaOptimalnuKupnju.Data;
using HelperZaOptimalnuKupnju.Models;
using HelperZaOptimalnuKupnju.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelperZaOptimalnuKupnju.Controllers
{
    [Route("proizv")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("lista")]
        public IActionResult Index(string search = "")
        {
            var query = _context.Products.Include(p => p.Stores).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            var products = query.ToList();
            ViewData["SearchTerm"] = search;
            return View(products);
        }

        [Route("pretraga")]
        [HttpGet]
        public IActionResult Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new { data = new object[] { } });
            }

            var results = _context.Products
                .Where(p => p.Name.Contains(term) || p.Description.Contains(term))
                .Select(p => new { id = p.Id, name = p.Name, price = p.UnitPrice })
                .Take(20)
                .ToList();

            return Json(new { data = results });
        }

        [Route("{id:int}")]
        [Route("detalji/{id:int}")]
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Stores)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();
            return View(product);
        }

        [Route("novo")]
        [HttpGet]
        [Authorize(Roles = "Admin,Supplier")]
        public IActionResult Create()
        {
            return View(new ProductCreateViewModel());
        }

        [Route("novo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Supplier")]
        public IActionResult Create(ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = new Product
                    {
                        Name = model.Name,
                        Description = model.Description,
                        UnitPrice = model.UnitPrice
                    };

                    _context.Products.Add(product);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Details), new { id = product.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Greška pri spremi podataka: {ex.Message}");
                }
            }

            return View(model);
        }

        [Route("uredi/{id:int}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Supplier")]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            var model = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                UnitPrice = product.UnitPrice
            };

            return View(model);
        }

        [Route("uredi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Supplier")]
        public IActionResult Edit(int id, ProductEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var product = _context.Products.Find(id);
                    if (product == null) return NotFound();

                    product.Name = model.Name;
                    product.Description = model.Description;
                    product.UnitPrice = model.UnitPrice;

                    _context.Products.Update(product);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Details), new { id = product.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Greška pri ažuriranju: {ex.Message}");
                }
            }

            return View(model);
        }

        [Route("obrisi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Supplier")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            try
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return BadRequest($"Ne mogu obrisati proizvod: {ex.Message}");
            }
        }
    }
}
