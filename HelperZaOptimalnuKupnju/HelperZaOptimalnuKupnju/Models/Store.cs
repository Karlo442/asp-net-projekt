using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HelperZaOptimalnuKupnju.Models
{
    public class Store
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty; // npr. "Lidl Krapina"

        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty; // npr. "Lidl", "Konzum"

        [Required]
        [StringLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string OpeningHours { get; set; } = string.Empty;

        // Navigacijsko svojstvo
        public virtual ICollection<Product>? Products { get; set; } = new List<Product>();
    }
}
