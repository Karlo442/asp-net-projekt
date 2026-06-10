using System;
using System.Linq;
using HelperZaOptimalnuKupnju.Data;
using HelperZaOptimalnuKupnju.DTOs;
using HelperZaOptimalnuKupnju.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelperZaOptimalnuKupnju.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Supplier")]
    public class ApiProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/apiProducts - Dohvata sve proizvode s opcionalnom pretragom
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductListDTO>>> GetProducts(
            [FromQuery] string? search = "",
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Products.AsQueryable();

                // Pretraga po nazivu ili opisu
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
                }

                // Filtriranje po cijeni
                if (minPrice.HasValue)
                {
                    query = query.Where(p => p.UnitPrice >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    query = query.Where(p => p.UnitPrice <= maxPrice.Value);
                }

                var total = query.Count();
                var products = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductListDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        UnitPrice = p.UnitPrice,
                        StoreCount = p.Stores != null ? p.Stores.Count : 0
                    })
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", total.ToString());
                Response.Headers.Add("X-Page-Number", pageNumber.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri dohvaćanju proizvoda", error = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/apiProducts/{id} - Dohvaća proizvod po ID-u sa svim detaljima
        /// </summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Stores)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound(new { message = $"Proizvod sa ID-om {id} nije pronađen" });
                }

                var dto = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    UnitPrice = product.UnitPrice,
                    Stores = product.Stores?.Select(s => new StoreDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Brand = s.Brand,
                        City = s.City
                    }).ToList()
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri dohvaćanju proizvoda", error = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/apiProducts - Kreira novi proizvod
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct(ProductCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    UnitPrice = dto.UnitPrice
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    UnitPrice = product.UnitPrice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri kreiranju proizvoda", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/apiProducts/{id} - Ažurira postojeći proizvod
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductEditDTO dto)
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

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = $"Proizvod sa ID-om {id} nije pronađen" });
                }

                product.Name = dto.Name;
                product.Description = dto.Description;
                product.UnitPrice = dto.UnitPrice;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Proizvod uspješno ažuriran" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri ažuriranju proizvoda", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE /api/apiProducts/{id} - Briše proizvod
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = $"Proizvod sa ID-om {id} nije pronađen" });
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Proizvod uspješno obrisan" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri brisanju proizvoda", error = ex.Message });
            }
        }
    }
}
