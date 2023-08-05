using OrderService.DTOs;

namespace OrderService.Services
{
    public interface IOrderService
    {
        Task<long> NewOrder(OrderDTO orderDTO, long buyerId);
        Task<OrderAllDTO> GetOrder(long orderId);
        Task<List<OrderAdminDTO>> GetOrders();
        Task<List<OrderAllDTO>> GetUndeliveredOrders(long userId);
        Task<List<OrderAllDTO>> GetNewOrders(long userId);
        Task<List<OrderAllDTO>> GetOldOrders(long userId);
        Task<List<OrderAllDTO>> GetUserOrders(long userId);
        Task<bool> CancelOrder(long orderId);
        Task<bool> ApproveOrder(long orderId);

        Task AddOrderItems(long orderId, List<OrderItemDTO> orderItems);

    }
}
