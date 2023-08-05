using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs;
using OrderService.Services;
using System.Data;
using System.Security.Claims;

namespace OrderService.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPut("new-order")]
        [Authorize(Roles = "BUYER")]
        public async Task<IActionResult> NewOrder([FromBody] OrderDTO orderDTO)
        {
            return Ok(await _orderService.NewOrder(orderDTO, GetUserIdFromToken(User)));
        }

        [HttpGet("get-order")]
        // [Authorize]
        public async Task<IActionResult> GetOrder(long id)
        {
            return Ok(await _orderService.GetOrder(id));
        }

        [HttpGet("get-orders")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetOrders()
        {
            return Ok(await _orderService.GetOrders());
        }

        [HttpGet("get-undelivered-orders")]
        [Authorize(Roles = "BUYER")]
        public async Task<IActionResult> GetUndeliveredOrders()
        {
            return Ok(await _orderService.GetUndeliveredOrders(GetUserIdFromToken(User)));
        }

        [HttpGet("get-seller-new-orders")]
        [Authorize(Roles = "SELLER")]
        public async Task<IActionResult> GetSellerNewOrders()
        {
            return Ok(await _orderService.GetNewOrders(GetUserIdFromToken(User)));
        }

        [HttpGet("get-seller-old-orders")]
        [Authorize(Roles = "SELLER")]
        public async Task<IActionResult> GetSellerOldOrders()
        {
            return Ok(await _orderService.GetOldOrders(GetUserIdFromToken(User)));
        }

        [HttpGet("get-user-orders")]
        [Authorize(Roles = "BUYER")]
        public async Task<IActionResult> GetUserOrders()
        {
            return Ok(await _orderService.GetUserOrders(GetUserIdFromToken(User)));
        }

        [HttpPut("cancel-order")]
        [Authorize(Roles = "BUYER")]
        public async Task<IActionResult> CancelOrder([FromForm] long orderId)
        {
            return Ok(await _orderService.CancelOrder(orderId));
        }
        [HttpPut("approve-order")]
        [Authorize(Roles = "SELLER")]
        public async Task<IActionResult> ApproveOrder([FromForm] long orderId)
        {
            return Ok(await _orderService.ApproveOrder(orderId));
        }

        [HttpPut("add-order-items")]
        [Authorize(Roles = "BUYER")]
        public async Task<IActionResult> AddItemsToOrder([FromBody] ItemsOrderDTO itemsOrderDTO)
        {
            await _orderService.AddOrderItems(itemsOrderDTO.OrderId, itemsOrderDTO.OrderItems);
            return Ok();
        }

        private long GetUserIdFromToken(ClaimsPrincipal user)
        {
            long id;
            long.TryParse(user.Identity.Name, out id);
            return id;
        }
    }
}
