using AutoMapper;
using UserService.DTOs;
using UserService.Models;

namespace UserService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.User, UserDTO>().ReverseMap();
            CreateMap<Models.User, UserLoginDTO>().ReverseMap();
            CreateMap<Models.User, UserRegisterDTO>().ReverseMap();
            CreateMap<Models.User, UserActivateDTO>().ReverseMap();
            CreateMap<Models.User, UserUpdateDTO>().ReverseMap();
            CreateMap<Models.User, ExternalUserDTO>().ReverseMap();
            CreateMap<Models.User, UserImageDTO>().ReverseMap();
            CreateMap<Models.User, UserPasswordDTO>().ReverseMap();
        }
    }
}
