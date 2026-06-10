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
    [Authorize(Roles = "Admin")]
    public class ApiOrderItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiOrderItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/apiOrderItems - Dohvaća sve stavke narudžbi s opcionalnom pretragom
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<OrderItemDTO>>> GetOrderItems(
            [FromQuery] int? orderId = null,
            [FromQuery] int? productId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.OrderItems
                    .Include(oi => oi.Product)
                    .AsQueryable();

                // Filtriranje po narudžbi
                if (orderId.HasValue)
                {
                    query = query.Where(oi => oi.OrderId == orderId.Value);
                }

                // Filtriranje po proizvodu
                if (productId.HasValue)
                {
                    query = query.Where(oi => oi.ProductId == productId.Value);
                }

                var total = query.Count();
                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(oi => new OrderItemDTO
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        ProductId = oi.ProductId,
                        Product = new ProductListDTO
                        {
                            Id = oi.Product.Id,
                            Name = oi.Product.Name,
                            UnitPrice = oi.Product.UnitPrice
                        },
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    })
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", total.ToString());
                Response.Headers.Add("X-Page-Number", pageNumber.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri dohvaćanju stavki narudžbi", error = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/apiOrderItems/{id} - Dohvaća stavku narudžbe po ID-u
        /// </summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<OrderItemDTO>> GetOrderItem(int id)
        {
            try
            {
                var item = await _context.OrderItems
                    .Include(oi => oi.Product)
                    .FirstOrDefaultAsync(oi => oi.Id == id);

                if (item == null)
                {
                    return NotFound(new { message = $"Stavka sa ID-om {id} nije pronađena" });
                }

                var dto = new OrderItemDTO
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    Product = new ProductListDTO
                    {
                        Id = item.Product.Id,
                        Name = item.Product.Name,
                        UnitPrice = item.Product.UnitPrice
                    },
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri dohvaćanju stavke narudžbe", error = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/apiOrderItems - Kreira novu stavku narudžbe
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrderItemDTO>> CreateOrderItem(OrderItemCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = await _context.Products.FindAsync(dto.ProductId);
                if (product == null)
                {
                    return BadRequest(new { message = $"Proizvod sa ID-om {dto.ProductId} nije pronađen" });
                }

                var item = new OrderItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = dto.UnitPrice
                };

                _context.OrderItems.Add(item);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrderItem), new { id = item.Id }, new OrderItemDTO
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri kreiranju stavke narudžbe", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/apiOrderItems/{id} - Ažurira stavku narudžbe
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOrderItem(int id, OrderItemEditDTO dto)
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

                var item = await _context.OrderItems.FindAsync(id);
                if (item == null)
                {
                    return NotFound(new { message = $"Stavka sa ID-om {id} nije pronađena" });
                }

                item.Quantity = dto.Quantity;
                item.UnitPrice = dto.UnitPrice;

                _context.OrderItems.Update(item);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Stavka narudžbe uspješno ažurirana" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri ažuriranju stavke narudžbe", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE /api/apiOrderItems/{id} - Briše stavku narudžbe
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            try
            {
                var item = await _context.OrderItems.FindAsync(id);
                if (item == null)
                {
                    return NotFound(new { message = $"Stavka sa ID-om {id} nije pronađena" });
                }

                _context.OrderItems.Remove(item);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Stavka narudžbe uspješno obrisana" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri brisanju stavke narudžbe", error = ex.Message });
            }
        }
    }
}
