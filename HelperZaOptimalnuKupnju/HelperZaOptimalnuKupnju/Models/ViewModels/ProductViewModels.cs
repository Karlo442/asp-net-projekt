using System.ComponentModel.DataAnnotations;

namespace HelperZaOptimalnuKupnju.Models.ViewModels
{
    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Naziv je obavezna polja")]
        [StringLength(200, ErrorMessage = "Naziv ne smije biti duži od 200 znakova")]
        [Display(Name = "Naziv proizvoda")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Opis je obavezan")]
        [StringLength(1000, ErrorMessage = "Opis ne smije biti duži od 1000 znakova")]
        [Display(Name = "Opis")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cijena je obavezna")]
        [Range(0.01, 999999.99, ErrorMessage = "Cijena mora biti između 0.01 i 999999.99")]
        [Display(Name = "Cijena po jedinici")]
        public decimal UnitPrice { get; set; }
    }

    public class ProductEditViewModel : ProductCreateViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
