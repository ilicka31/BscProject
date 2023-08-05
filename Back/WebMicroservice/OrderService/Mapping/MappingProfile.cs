using AutoMapper;
using OrderService.DTOs;
using OrderService.Models;

namespace OrderService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<Order, OrderAllDTO>().ReverseMap();
            CreateMap<Order, OrderAdminDTO>().ReverseMap();

            CreateMap<OrderItem, OrderItemDTO>().ReverseMap();

        }
    }
}
