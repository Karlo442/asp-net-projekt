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
    public class ApiUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/apiUsers - Dohvaća sve korisnike s opcionalnom pretragom
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UserListDTO>>> GetUsers(
            [FromQuery] string? search = "",
            [FromQuery] string? role = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                // Pretraga po imenu ili emailu
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(u => u.FirstName.Contains(search) || u.LastName.Contains(search) || u.Email.Contains(search));
                }

                // Filtriranje po ulozi
                if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, true, out var userRole))
                {
                    query = query.Where(u => u.Role == userRole);
                }

                // Filtriranje po statusu aktivnosti
                if (isActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == isActive.Value);
                }

                var total = query.Count();
                var users = await query
                    .OrderBy(u => u.LastName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserListDTO
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Role = u.Role,
                        IsActive = u.IsActive
                    })
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", total.ToString());
                Response.Headers.Add("X-Page-Number", pageNumber.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri dohvaćanju korisnika", error = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/apiUsers/{id} - Dohvaća korisnika po ID-u
        /// </summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(new { message = $"Korisnik sa ID-om {id} nije pronađen" });
                }

                var dto = new UserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                    Phone = user.Phone,
                    RegisteredAt = user.RegisteredAt,
                    IsActive = user.IsActive
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri dohvaćanju korisnika", error = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/apiUsers - Kreira novog korisnika
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser(UserCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Provjera da li email već postoji
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = $"Korisnik sa emailom '{dto.Email}' već postoji" });
                }

                var user = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Role = dto.Role,
                    Phone = dto.Phone,
                    RegisteredAt = DateTime.Now,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                    Phone = user.Phone,
                    RegisteredAt = user.RegisteredAt,
                    IsActive = user.IsActive
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri kreiranju korisnika", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/apiUsers/{id} - Ažurira korisnika
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, UserEditDTO dto)
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

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = $"Korisnik sa ID-om {id} nije pronađen" });
                }

                // Provjera da li email već postoji (osim za trenutnog korisnika)
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.Id != id);
                if (existingUser != null)
                {
                    return BadRequest(new { message = $"Korisnik sa emailom '{dto.Email}' već postoji" });
                }

                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Email = dto.Email;
                user.Role = dto.Role;
                user.Phone = dto.Phone;
                user.IsActive = dto.IsActive;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Korisnik uspješno ažuriran" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri ažuriranju korisnika", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE /api/apiUsers/{id} - Briše korisnika
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = $"Korisnik sa ID-om {id} nije pronađen" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Korisnik uspješno obrisan" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri brisanju korisnika", error = ex.Message });
            }
        }
    }
}
