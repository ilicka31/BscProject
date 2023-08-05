using AutoMapper;
using ProductService.DTOs;
using ProductService.Models;

namespace ProductService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, ProductUpdateDTO>().ReverseMap();
            CreateMap<Product, ProductGetDTO>().ReverseMap();
            CreateMap<Product, ProductImageDTO>().ReverseMap();
            CreateMap<Product, ProductUploadImageDTO>().ReverseMap();
        }
    }
}
