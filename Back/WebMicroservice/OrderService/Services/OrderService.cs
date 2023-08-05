using AutoMapper;
using Common.Entites;
using Common.Entities.Product;
using Common.Exceptions;
using OrderService.DTOs;
using OrderService.Models;
using OrderService.Repository;

namespace OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderService(IMapper mapper, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<bool> ApproveOrder(long orderId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null) { throw new NotFoundException("Order doesn't exist."); }

            Random random = new Random();
            int minutes = random.Next(65, 180);
            order.DeliveryDate = DateTime.Now.AddMinutes(minutes);
            order.Approved = true;
            _orderRepository.Update(order);

         

            return true;
        }
        private async Task<List<OrderItem>> GetItemsForOrderId(long id)
        {
            var l= await _orderItemRepository.GetAll();
            List<OrderItem> lista = l.ToList();
            return lista.Where(oi => oi.OrderId == id).ToList();
        }
        public async Task<bool> CancelOrder(long orderId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null) { throw new NotFoundException("Order doesn't exist."); }
            order.OrderItems = await GetItemsForOrderId(orderId);

            List<ProductInfo> list = await _orderRepository.GetAllProducts();

            foreach (var item in order.OrderItems)
            {
                foreach (var product in list)
                {
                    if (item.ProductId == product.Id)
                    {
                        product.MaxQuantity = product.MaxQuantity + item.Quantity;
                        _orderRepository.UpdateProduct(product);
                    }
                }
            }

            order.Confirmed = false;
            _orderRepository.Update(order);
            return true;
        }

        public async Task<List<OrderAllDTO>> GetNewOrders(long userId)
        {
            var user = await _orderRepository.GetUser(userId);

            if (user == null) { throw new NotFoundException("User doesn't exist."); }
            var sellerOrders = await _orderRepository.GetSellerOrders(userId, false);
            foreach (var sellerOrder in sellerOrders)
            {
                var pom = await GetItemsForOrderId(sellerOrder.Id);
                sellerOrder.OrderItems = pom.FindAll(x => x.SellerId == userId);
            }
            return _mapper.Map<List<OrderAllDTO>>(sellerOrders);
        }

        public async Task<List<OrderAllDTO>> GetOldOrders(long userId)
        {
            var user = await _orderRepository.GetUser(userId);

            if (user == null) { throw new NotFoundException("User doesn't exist"); }
            var sellerOrders = await _orderRepository.GetSellerOrders(userId, true);
            foreach (var sellerOrder in sellerOrders)
            {
                var pom = await GetItemsForOrderId(sellerOrder.Id);
                sellerOrder.OrderItems = pom.FindAll(x => x.SellerId == userId);
            }
            return _mapper.Map<List<OrderAllDTO>>(sellerOrders);

        }

        public async Task<OrderAllDTO> GetOrder(long orderId)
        {
            Order order = await _orderRepository.GetById(orderId);
            if (order == null)
                return null;
            order.OrderItems = await GetItemsForOrderId(orderId);
            return _mapper.Map<OrderAllDTO>(order);
        }

        public async Task<List<OrderAdminDTO>> GetOrders()
        {
            var orders = await _orderRepository.GetAll();
            foreach (var order in orders)
            {
                order.OrderItems = await GetItemsForOrderId(order.Id);
            }
            return _mapper.Map<List<OrderAdminDTO>>(orders);
        }

        public async Task<List<OrderAllDTO>> GetUndeliveredOrders(long userId)
        {
            var user = await _orderRepository.GetUser(userId);

            if (user == null) { throw new NotFoundException("User doesn't exist."); }
            var l = await _orderRepository.GetAll();
            List<Order> lista = l.ToList();
             var userOrders = lista.Where(o => o.BuyerId == user.Id).ToList();
            foreach (var order in userOrders)
            {
                order.OrderItems = await GetItemsForOrderId(order.Id);
            }
            return _mapper.Map<List<OrderAllDTO>>(userOrders.Where(order => order.DeliveryDate > DateTime.Now && order.Confirmed));

        }

        public async Task<List<OrderAllDTO>> GetUserOrders(long userId)
        {
            var user = await _orderRepository.GetUser(userId);

            if (user == null) { throw new NotFoundException("User doesn't exist."); }
            var l= await _orderRepository.GetAll();
            List<Order> lista = l.ToList();
            var userOrders = lista.Where(o => o.BuyerId == user.Id).ToList();
            foreach (var order in userOrders)
            {
                order.OrderItems = await GetItemsForOrderId(order.Id);
            }
           
            return _mapper.Map<List<OrderAllDTO>>(userOrders.Where(order => order.DeliveryDate < DateTime.Now && order.Confirmed));

        }

        public async Task<long> NewOrder(OrderDTO orderDTO, long buyerId)
        {
            var user = await _orderRepository.GetUser(buyerId);
            if (user == null) { throw new NotFoundException("User doesn't exist."); }

            bool isPayed = orderDTO.IsPayed;
            Order newOrder = _mapper.Map<Order>(orderDTO);

            newOrder.Confirmed = true;
            if (isPayed)
            {
                newOrder.Approved = false;
                newOrder.Address = user.Address;
                newOrder.DeliveryDate = DateTime.Now.AddMinutes(10);
            }
            else
            {
                newOrder.Approved = true;
                Random random = new Random();
                int minutes = random.Next(65, 180);
                newOrder.DeliveryDate = DateTime.Now.AddMinutes(minutes);
            }
            newOrder.BuyerId = user.Id;

            await _orderRepository.Create(newOrder);
          

            var lastOrder = await _orderRepository.GetNewestOrderId();


            return lastOrder;
        }

        public async Task AddOrderItems(long orderId, List<OrderItemDTO> orderItems)
        {
            Order o = await _orderRepository.GetById(orderId);
            if (o == null) { throw new ConflictException("Order doesn't exist!"); }

            List<ProductInfo> productList = await _orderRepository.GetAllProducts();
            List<OrderItem> orders = new List<OrderItem>();

            foreach (var oItem in orderItems)
            {
                orders.Add(_mapper.Map<OrderItem>(oItem));
            }

            // await _unitOfWork.Save();
            List<UserInfo> allSellers = await _orderRepository.GetAllUsers();
            allSellers = allSellers.Where(s => s.UserType == "SELLER").ToList();


            foreach (var item in productList)
            {
                foreach (var oItem in orders)
                {
                    oItem.OrderId = orderId;
                    if (oItem.ProductId == item.Id)
                    {
                        oItem.Name = item.Name;
                        oItem.Price = item.Price;
                        oItem.SellerId = item.SellerId;
                    }
                }
            }
            o.OrderItems = orders;
            foreach (var item in orders)
            {
                await _orderItemRepository.Create(item);
            }

            List<long> ids = new List<long>();
            List<UserInfo> sellers = new List<UserInfo>();

            foreach (var item in o.OrderItems)
            {
                ids.Add(item.SellerId);
                foreach (var product in productList)
                {
                    if (item.ProductId == product.Id)
                    {
                        if (product.MaxQuantity >= item.Quantity)
                        {
                            product.MaxQuantity = product.MaxQuantity - item.Quantity;
                           await _orderRepository.UpdateProduct(product);
                        }
                    }
                }
            }
            foreach (var seller in allSellers)
            {
                if (ids.Contains(seller.Id))
                {
                    if (!sellers.Contains(seller))
                    {
                        sellers.Add(seller);
                    }
                }
            }
            o.TotalPrice += 230 * sellers.Count();

           await _orderRepository.Update(o);
           
        }
    }
}
