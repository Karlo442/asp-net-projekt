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
    public class ApiOrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/apiOrders - Dohvaća sve narudžbe s opcionalnom pretragom
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<OrderListDTO>>> GetOrders(
            [FromQuery] string? search = "",
            [FromQuery] int? buyerId = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Orders.Include(o => o.Buyer).AsQueryable();

                // Filtriranje po kupcu
                if (buyerId.HasValue)
                {
                    query = query.Where(o => o.BuyerId == buyerId.Value);
                }

                // Filtriranje po statusu
                if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
                {
                    query = query.Where(o => o.Status == orderStatus);
                }

                // Pretraga po imenu kupca
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(o => o.Buyer.FirstName.Contains(search) || o.Buyer.LastName.Contains(search));
                }

                var total = query.Count();
                var orders = await query
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(o => new OrderListDTO
                    {
                        Id = o.Id,
                        BuyerId = o.BuyerId,
                        BuyerName = $"{o.Buyer.FirstName} {o.Buyer.LastName}",
                        CreatedAt = o.CreatedAt,
                        Status = o.Status,
                        TotalAmount = o.TotalAmount,
                        ItemCount = o.Items != null ? o.Items.Count : 0
                    })
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", total.ToString());
                Response.Headers.Add("X-Page-Number", pageNumber.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri dohvaćanju narudžbi", error = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/apiOrders/{id} - Dohvaća narudžbu po ID-u sa svim detaljima
        /// </summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Buyer)
                    .Include(o => o.Items!)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return NotFound(new { message = $"Narudžba sa ID-om {id} nije pronađena" });
                }

                var dto = new OrderDTO
                {
                    Id = order.Id,
                    BuyerId = order.BuyerId,
                    Buyer = new UserDTO
                    {
                        Id = order.Buyer.Id,
                        FirstName = order.Buyer.FirstName,
                        LastName = order.Buyer.LastName,
                        Email = order.Buyer.Email
                    },
                    CreatedAt = order.CreatedAt,
                    ExpectedDeliveryDateTime = order.ExpectedDeliveryDateTime,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    Items = order.Items?.Select(oi => new OrderItemDTO
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
                    }).ToList()
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri dohvaćanju narudžbe", error = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/apiOrders - Kreira novu narudžbu
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<OrderDTO>> CreateOrder(OrderCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Provjera da li kupac postoji
                var buyer = await _context.Users.FindAsync(dto.BuyerId);
                if (buyer == null)
                {
                    return BadRequest(new { message = $"Kupac sa ID-om {dto.BuyerId} nije pronađen" });
                }

                var order = new Order
                {
                    BuyerId = dto.BuyerId,
                    CreatedAt = DateTime.Now,
                    ExpectedDeliveryDateTime = dto.ExpectedDeliveryDateTime,
                    Status = OrderStatus.Pending,
                    TotalAmount = 0,
                    Items = new List<OrderItem>()
                };

                // Dodavanje stavki u narudžbu
                decimal total = 0;
                foreach (var itemDto in dto.Items)
                {
                    var product = await _context.Products.FindAsync(itemDto.ProductId);
                    if (product == null)
                    {
                        return BadRequest(new { message = $"Proizvod sa ID-om {itemDto.ProductId} nije pronađen" });
                    }

                    var orderItem = new OrderItem
                    {
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        UnitPrice = product.UnitPrice
                    };

                    order.Items.Add(orderItem);
                    total += orderItem.Quantity * orderItem.UnitPrice;
                }

                order.TotalAmount = total;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new OrderDTO
                {
                    Id = order.Id,
                    BuyerId = order.BuyerId,
                    CreatedAt = order.CreatedAt,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri kreiranju narudžbe", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/apiOrders/{id} - Ažurira postojeću narudžbu
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> UpdateOrder(int id, OrderEditDTO dto)
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

                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound(new { message = $"Narudžba sa ID-om {id} nije pronađena" });
                }

                order.ExpectedDeliveryDateTime = dto.ExpectedDeliveryDateTime;
                order.Status = dto.Status;

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Narudžba uspješno ažurirana" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri ažuriranju narudžbe", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE /api/apiOrders/{id} - Briše narudžbu
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items!)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return NotFound(new { message = $"Narudžba sa ID-om {id} nije pronađena" });
                }

                if (order.Items?.Any() == true)
                {
                    _context.OrderItems.RemoveRange(order.Items);
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Narudžba uspješno obrisana" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri brisanju narudžbe", error = ex.Message });
            }
        }
    }
}
