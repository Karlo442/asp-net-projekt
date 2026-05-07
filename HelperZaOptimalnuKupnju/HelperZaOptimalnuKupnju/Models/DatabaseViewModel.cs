namespace HelperZaOptimalnuKupnju.Models
{
    public class DatabaseViewModel
    {
        public int Users { get; set; }
        public int Products { get; set; }
        public int Orders { get; set; }
        public int Stores { get; set; }
        public int OrderItems { get; set; }
        public string DbPath { get; set; } = string.Empty;
    }
}
