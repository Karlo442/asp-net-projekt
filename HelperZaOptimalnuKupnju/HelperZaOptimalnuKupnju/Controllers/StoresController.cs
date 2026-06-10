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
    [Route("trgov")]
    public class StoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("sve")]
        public IActionResult Index(string search = "")
        {
            var query = _context.Stores.Include(s => s.Products).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => 
                    s.Name.Contains(search) || 
                    s.Brand.Contains(search) ||
                    s.City.Contains(search));
            }

            var stores = query.ToList();
            ViewData["SearchTerm"] = search;
            return View(stores);
        }

        [Route("pretraga")]
        [HttpGet]
        public IActionResult Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new { data = new object[] { } });
            }

            var results = _context.Stores
                .Where(s => 
                    s.Name.Contains(term) || 
                    s.Brand.Contains(term) ||
                    s.City.Contains(term))
                .Select(s => new 
                { 
                    id = s.Id, 
                    name = s.Name,
                    brand = s.Brand,
                    city = s.City,
                    address = s.Address
                })
                .Take(20)
                .ToList();

            return Json(new { data = results });
        }

        [Route("{id:int}")]
        [Route("info/{id:int}")]
        public IActionResult Details(int id)
        {
            var store = _context.Stores
                .Include(s => s.Products)
                .FirstOrDefault(s => s.Id == id);

            if (store == null) return NotFound();
            return View(store);
        }

        [Route("novo")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new StoreCreateViewModel());
        }

        [Route("novo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StoreCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var store = new Store
                    {
                        Name = model.Name,
                        Brand = model.Brand,
                        Address = model.Address,
                        City = model.City,
                        Country = model.Country,
                        OpeningHours = model.OpeningHours
                    };

                    _context.Stores.Add(store);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Details), new { id = store.Id });
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
        public IActionResult Edit(int id)
        {
            var store = _context.Stores.Find(id);
            if (store == null) return NotFound();

            var model = new StoreEditViewModel
            {
                Id = store.Id,
                Name = store.Name,
                Brand = store.Brand,
                Address = store.Address,
                City = store.City,
                Country = store.Country,
                OpeningHours = store.OpeningHours
            };

            return View(model);
        }

        [Route("uredi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, StoreEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var store = _context.Stores.Find(id);
                    if (store == null) return NotFound();

                    store.Name = model.Name;
                    store.Brand = model.Brand;
                    store.Address = model.Address;
                    store.City = model.City;
                    store.Country = model.Country;
                    store.OpeningHours = model.OpeningHours;

                    _context.Stores.Update(store);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Details), new { id = store.Id });
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
        public IActionResult Delete(int id)
        {
            var store = _context.Stores.Include(s => s.Products).FirstOrDefault(s => s.Id == id);
            if (store == null) return NotFound();

            try
            {
                if (store.Products != null && store.Products.Any())
                {
                    return BadRequest("Ne mogu obrisati dućan koji ima proizvode.");
                }

                _context.Stores.Remove(store);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return BadRequest($"Ne mogu obrisati dućan: {ex.Message}");
            }
        }
    }
}
