using System;
using System.ComponentModel.DataAnnotations;

namespace HelperZaOptimalnuKupnju.Models.ViewModels
{
    public class StoreCreateViewModel
    {
        [Required(ErrorMessage = "Naziv je obavezan")]
        [StringLength(200, ErrorMessage = "Naziv ne smije biti duži od 200 znakova")]
        [Display(Name = "Naziv dućana")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Marka je obavezna")]
        [StringLength(100, ErrorMessage = "Marka ne smije biti duža od 100 znakova")]
        [Display(Name = "Marka")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adresa je obavezna")]
        [StringLength(300, ErrorMessage = "Adresa ne smije biti duža od 300 znakova")]
        [Display(Name = "Adresa")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grad je obavezan")]
        [StringLength(100, ErrorMessage = "Grad ne smije biti duži od 100 znakova")]
        [Display(Name = "Grad")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Država je obavezna")]
        [StringLength(100, ErrorMessage = "Država ne smije biti duža od 100 znakova")]
        [Display(Name = "Država")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "Radno vrijeme je obavezno")]
        [StringLength(100, ErrorMessage = "Radno vrijeme ne smije biti duže od 100 znakova")]
        [Display(Name = "Radno vrijeme")]
        public string OpeningHours { get; set; } = string.Empty;
    }

    public class StoreEditViewModel : StoreCreateViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
