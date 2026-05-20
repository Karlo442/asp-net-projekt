using System;
using System.ComponentModel.DataAnnotations;

namespace HelperZaOptimalnuKupnju.Models.ViewModels
{
    public class OrderCreateViewModel
    {
        [Required(ErrorMessage = "Kupac je obavezan")]
        [Display(Name = "Kupac")]
        public int BuyerId { get; set; }

        [Required(ErrorMessage = "Očekivani datum dostave je obavezan")]
        [Display(Name = "Očekivani datum dostave")]
        public DateTime ExpectedDeliveryDateTime { get; set; }

        [Required(ErrorMessage = "Status je obavezan")]
        [Display(Name = "Status")]
        public OrderStatus Status { get; set; }
    }

    public class OrderEditViewModel : OrderCreateViewModel
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Datum kreiranja")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Ukupan iznos")]
        public decimal TotalAmount { get; set; }
    }

    public class OrderSearchViewModel
    {
        public int Id { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
