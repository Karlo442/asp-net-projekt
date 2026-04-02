namespace HelperZaOptimalnuKupnju.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public User Supplier { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }
}
