using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelperZaOptimalnuKupnju.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        [Required]
        [Phone]
        [StringLength(50, ErrorMessage = "Telefon ne smije sadržavati više od 50 znakova.")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public DateTime RegisteredAt { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public virtual ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}
