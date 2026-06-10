namespace HelperZaOptimalnuKupnju.DTOs
{
    public class StoreDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string OpeningHours { get; set; } = string.Empty;
        public List<ProductListDTO>? Products { get; set; }
    }

    public class StoreCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string OpeningHours { get; set; } = string.Empty;
    }

    public class StoreEditDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string OpeningHours { get; set; } = string.Empty;
    }

    public class StoreListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}
