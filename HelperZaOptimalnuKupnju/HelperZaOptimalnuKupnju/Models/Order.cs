using System;
using System.Collections.Generic;

namespace HelperZaOptimalnuKupnju.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public User Buyer { get; set; }
        public int SupplierId { get; set; }
        public User Supplier { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpectedDeliveryDateTime { get; set; }
        public OrderStatus Status { get; set; }
        public ICollection<OrderItem>? Items { get; set; }
        
    }
}
