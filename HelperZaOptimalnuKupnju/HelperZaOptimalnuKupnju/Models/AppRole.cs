using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HelperZaOptimalnuKupnju.Models
{
    public class AppRole : IdentityRole
    {
        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
