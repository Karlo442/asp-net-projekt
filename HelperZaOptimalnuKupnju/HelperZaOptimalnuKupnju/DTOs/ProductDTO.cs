using System.ComponentModel.DataAnnotations;

namespace HelperZaOptimalnuKupnju.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public List<StoreDTO>? Stores { get; set; }
    }

    public class ProductCreateDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }

    public class ProductEditDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }

    public class ProductListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int StoreCount { get; set; }
    }
}
