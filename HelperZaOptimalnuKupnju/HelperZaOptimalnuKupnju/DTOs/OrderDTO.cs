using HelperZaOptimalnuKupnju.Models;

namespace HelperZaOptimalnuKupnju.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public UserDTO? Buyer { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpectedDeliveryDateTime { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDTO>? Items { get; set; }
    }

    public class OrderCreateDTO
    {
        public int BuyerId { get; set; }
        public DateTime ExpectedDeliveryDateTime { get; set; }
        public List<OrderItemCreateDTO> Items { get; set; } = new();
    }

    public class OrderEditDTO
    {
        public int Id { get; set; }
        public DateTime ExpectedDeliveryDateTime { get; set; }
        public OrderStatus Status { get; set; }
    }

    public class OrderListDTO
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public string? BuyerName { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
}
