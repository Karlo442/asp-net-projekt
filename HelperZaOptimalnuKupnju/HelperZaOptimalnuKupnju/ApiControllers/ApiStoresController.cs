using System;
using System.Linq;
using HelperZaOptimalnuKupnju.Data;
using HelperZaOptimalnuKupnju.DTOs;
using HelperZaOptimalnuKupnju.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelperZaOptimalnuKupnju.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiStoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiStoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/apiStores - Dohvaća sve trgovine s opcionalnom pretragom
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoreListDTO>>> GetStores(
            [FromQuery] string? search = "",
            [FromQuery] string? brand = null,
            [FromQuery] string? city = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Stores.Include(s => s.Products).AsQueryable();

                // Pretraga po nazivu ili adresi
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(s => s.Name.Contains(search) || s.Address.Contains(search));
                }

                // Filtriranje po brandu
                if (!string.IsNullOrWhiteSpace(brand))
                {
                    query = query.Where(s => s.Brand.Contains(brand));
                }

                // Filtriranje po gradu
                if (!string.IsNullOrWhiteSpace(city))
                {
                    query = query.Where(s => s.City.Contains(city));
                }

                var total = query.Count();
                var stores = await query
                    .OrderBy(s => s.Brand)
                    .ThenBy(s => s.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new StoreListDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Brand = s.Brand,
                        City = s.City,
                        ProductCount = s.Products != null ? s.Products.Count : 0
                    })
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", total.ToString());
                Response.Headers.Add("X-Page-Number", pageNumber.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(stores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri dohvaćanju trgovina", error = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/apiStores/{id} - Dohvaća trgovinu po ID-u sa svim detaljima
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StoreDTO>> GetStore(int id)
        {
            try
            {
                var store = await _context.Stores
                    .Include(s => s.Products)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (store == null)
                {
                    return NotFound(new { message = $"Trgovina sa ID-om {id} nije pronađena" });
                }

                var dto = new StoreDTO
                {
                    Id = store.Id,
                    Name = store.Name,
                    Brand = store.Brand,
                    Address = store.Address,
                    City = store.City,
                    Country = store.Country,
                    OpeningHours = store.OpeningHours,
                    Products = store.Products?.Select(p => new ProductListDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        UnitPrice = p.UnitPrice
                    }).ToList()
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri dohvaćanju trgovine", error = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/apiStores - Kreira novu trgovinu
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<StoreDTO>> CreateStore(StoreCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var store = new Store
                {
                    Name = dto.Name,
                    Brand = dto.Brand,
                    Address = dto.Address,
                    City = dto.City,
                    Country = dto.Country,
                    OpeningHours = dto.OpeningHours
                };

                _context.Stores.Add(store);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetStore), new { id = store.Id }, new StoreDTO
                {
                    Id = store.Id,
                    Name = store.Name,
                    Brand = store.Brand,
                    Address = store.Address,
                    City = store.City,
                    Country = store.Country,
                    OpeningHours = store.OpeningHours
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri kreiranju trgovine", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/apiStores/{id} - Ažurira postojeću trgovinu
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStore(int id, StoreEditDTO dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest(new { message = "ID-evi se ne poklapaju" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var store = await _context.Stores.FindAsync(id);
                if (store == null)
                {
                    return NotFound(new { message = $"Trgovina sa ID-om {id} nije pronađena" });
                }

                store.Name = dto.Name;
                store.Brand = dto.Brand;
                store.Address = dto.Address;
                store.City = dto.City;
                store.Country = dto.Country;
                store.OpeningHours = dto.OpeningHours;

                _context.Stores.Update(store);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Trgovina uspješno ažurirana" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri ažuriranju trgovine", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE /api/apiStores/{id} - Briše trgovinu
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            try
            {
                var store = await _context.Stores.FindAsync(id);
                if (store == null)
                {
                    return NotFound(new { message = $"Trgovina sa ID-om {id} nije pronađena" });
                }

                _context.Stores.Remove(store);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Trgovina uspješno obrisana" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri brisanju trgovine", error = ex.Message });
            }
        }
    }
}
