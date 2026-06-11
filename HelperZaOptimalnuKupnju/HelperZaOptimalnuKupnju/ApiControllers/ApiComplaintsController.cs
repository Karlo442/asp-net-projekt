using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelperZaOptimalnuKupnju.Data;
using HelperZaOptimalnuKupnju.DTOs;
using HelperZaOptimalnuKupnju.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelperZaOptimalnuKupnju.ApiControllers
{
    [ApiController]
    [Route("api/complaints")]
    [Authorize]
    [IgnoreAntiforgeryToken]
    public class ApiComplaintsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private const string UploadFolder = "uploads/complaints";

        public ApiComplaintsController(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        /// <summary>
        /// GET /api/complaints - Dohvata sve žalbe trenutnog korisnika
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComplaintDTO>>> GetUserComplaints()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var complaints = await _context.Complaints
                    .Where(c => c.UserId == user.Id)
                    .Select(c => new ComplaintDTO
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Description = c.Description,
                        FilePath = c.FilePath,
                        OriginalFileName = c.OriginalFileName,
                        ContentType = c.ContentType,
                        FileSize = c.FileSize,
                        CreatedAt = c.CreatedAt,
                        UserName = user.UserName ?? "Nepoznat"
                    })
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Greška: {ex.Message}" });
            }
        }

        /// <summary>
        /// POST /api/complaints - Uploadaj novu žalbu s datotekom
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ComplaintDTO>> CreateComplaint(
            [FromForm] string title,
            [FromForm] string description,
            [FromForm] IFormFile? file)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
                    return BadRequest(new { message = "Naslov i opis su obavezni." });

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var complaint = new Complaint
                {
                    Title = title.Trim(),
                    Description = description.Trim(),
                    CreatedAt = DateTime.Now,
                    UserId = user.Id
                };

                // Ako je datoteka uploadana
                if (file != null && file.Length > 0)
                {
                    var uploadsPath = Path.Combine(_environment.WebRootPath, UploadFolder);
                    Directory.CreateDirectory(uploadsPath);

                    var fileName = $"{user.Id}_{DateTime.Now:yyyyMMdd_HHmmss}_{Path.GetFileName(file.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    complaint.FilePath = Path.Combine(UploadFolder, fileName).Replace("\\", "/");
                    complaint.OriginalFileName = Path.GetFileName(file.FileName);
                    complaint.ContentType = file.ContentType;
                    complaint.FileSize = file.Length;
                }

                _context.Complaints.Add(complaint);
                await _context.SaveChangesAsync();

                var dto = new ComplaintDTO
                {
                    Id = complaint.Id,
                    Title = complaint.Title,
                    Description = complaint.Description,
                    FilePath = complaint.FilePath,
                    OriginalFileName = complaint.OriginalFileName,
                    ContentType = complaint.ContentType,
                    FileSize = complaint.FileSize,
                    CreatedAt = complaint.CreatedAt,
                    UserName = user.UserName ?? "Nepoznat"
                };

                return CreatedAtAction(nameof(GetUserComplaints), new { id = complaint.Id }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Greška pri uploadu: {ex.Message}" });
            }
        }

        /// <summary>
        /// DELETE /api/complaints/{id} - Obriši žalbu i njenu datoteku
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplaint(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var complaint = await _context.Complaints.FindAsync(id);
                if (complaint == null)
                    return NotFound(new { message = "Žalba nije pronađena." });

                // Provjera vlasništva
                if (complaint.UserId != user.Id && !User.IsInRole("Admin"))
                    return Forbid();

                // Obriši datoteku s diska
                if (!string.IsNullOrWhiteSpace(complaint.FilePath))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, complaint.FilePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Complaints.Remove(complaint);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Žalba je obrisana." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Greška pri brisanju: {ex.Message}" });
            }
        }
    }
}
