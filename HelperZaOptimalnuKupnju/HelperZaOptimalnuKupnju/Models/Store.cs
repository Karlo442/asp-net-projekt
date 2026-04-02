using System.Collections.Generic;

namespace HelperZaOptimalnuKupnju.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // npr. "Lidl Krapina"
        public string Brand { get; set; } = string.Empty; // npr. "Lidl", "Konzum"
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string OpeningHours { get; set; } = string.Empty;

        // Navigacijsko svojstvo
        public ICollection<Product>? Products { get; set; }
    }
}
