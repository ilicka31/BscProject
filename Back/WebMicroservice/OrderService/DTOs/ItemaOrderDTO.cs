namespace OrderService.DTOs
{
    public class ItemsOrderDTO
    {
        public long OrderId { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
    }
}
