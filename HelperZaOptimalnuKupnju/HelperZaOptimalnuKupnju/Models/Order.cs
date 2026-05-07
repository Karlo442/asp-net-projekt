using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelperZaOptimalnuKupnju.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BuyerId { get; set; }

        [ForeignKey(nameof(BuyerId))]
        [Required]
        public virtual User Buyer { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ExpectedDeliveryDateTime { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public virtual ICollection<OrderItem>? Items { get; set; } = new List<OrderItem>();
    }
}
