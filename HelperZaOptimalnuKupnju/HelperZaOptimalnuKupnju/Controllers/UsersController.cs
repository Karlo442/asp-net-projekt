using System;
using System.Linq;
using HelperZaOptimalnuKupnju.Data;
using HelperZaOptimalnuKupnju.Models;
using HelperZaOptimalnuKupnju.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelperZaOptimalnuKupnju.Controllers
{
    [Route("korisn")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("lista")]
        public IActionResult Index(string search = "")
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => 
                    u.FirstName.Contains(search) || 
                    u.LastName.Contains(search) ||
                    u.Email.Contains(search));
            }

            var users = query.ToList();
            ViewData["SearchTerm"] = search;
            return View(users);
        }

        [Route("pretraga")]
        [HttpGet]
        public IActionResult Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new { data = new object[] { } });
            }

            var results = _context.Users
                .Where(u => 
                    u.FirstName.Contains(term) || 
                    u.LastName.Contains(term) ||
                    u.Email.Contains(term))
                .Select(u => new 
                { 
                    id = u.Id, 
                    name = u.FirstName + " " + u.LastName,
                    email = u.Email,
                    role = u.Role.ToString(),
                    isActive = u.IsActive
                })
                .Take(20)
                .ToList();

            return Json(new { data = results });
        }

        [Route("{id:int}")]
        [Route("profil/{id:int}")]
        public IActionResult Details(int id)
        {
            var user = _context.Users
                .Include(u => u.Orders)
                .FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();
            return View(user);
        }

        [Route("novo")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserCreateViewModel());
        }

        [Route("novo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.Users.Any(u => u.Email == model.Email))
                    {
                        ModelState.AddModelError("Email", "Email na serveru je već registriran.");
                        return View(model);
                    }

                    var user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone,
                        Role = model.Role,
                        IsActive = model.IsActive,
                        RegisteredAt = DateTime.Now
                    };

                    _context.Users.Add(user);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Details), new { id = user.Id });
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
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            var model = new UserEditViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                IsActive = user.IsActive
            };

            return View(model);
        }

        [Route("uredi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, UserEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _context.Users.Find(id);
                    if (user == null) return NotFound();

                    if (user.Email != model.Email && _context.Users.Any(u => u.Email == model.Email))
                    {
                        ModelState.AddModelError("Email", "Email je već registriran.");
                        return View(model);
                    }

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.Phone = model.Phone;
                    user.Role = model.Role;
                    user.IsActive = model.IsActive;

                    _context.Users.Update(user);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Details), new { id = user.Id });
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
            var user = _context.Users.Include(u => u.Orders).FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            try
            {
                if (user.Orders != null && user.Orders.Any())
                {
                    return BadRequest("Ne mogu obrisati korisnika koji ima narudžbe.");
                }

                _context.Users.Remove(user);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return BadRequest($"Ne mogu obrisati korisnika: {ex.Message}");
            }
        }
    }
}
