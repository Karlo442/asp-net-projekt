using System;
using System.ComponentModel.DataAnnotations;

namespace HelperZaOptimalnuKupnju.Models.ViewModels
{
    public class UserCreateViewModel
    {
        [Required(ErrorMessage = "Ime je obavezno")]
        [StringLength(120, ErrorMessage = "Ime ne smije biti duže od 100 znakova")]
        [Display(Name = "Ime")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno")]
        [StringLength(100, ErrorMessage = "Prezime ne smije biti duže od 100 znakova")]
        [Display(Name = "Prezime")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Unesite ispravan email")]
        [StringLength(200)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon je obavezan")]
        [Phone(ErrorMessage = "Unesite ispravan broj telefona")]
        [StringLength(50)]
        [Display(Name = "Telefon")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Uloga je obavezna")]
        [Display(Name = "Uloga")]
        public UserRole Role { get; set; }

        [Display(Name = "Aktivna")]
        public bool IsActive { get; set; } = true;
    }

    public class UserEditViewModel : UserCreateViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
