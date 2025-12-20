using System;
using AutoMapper;
using WebApiEcommerce.Model;
using WebApiEcommerce.Model.Dtos;

namespace WebApiEcommerce.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, CreateUserDto>().ReverseMap();
        CreateMap<User, UpdateUserDto>().ReverseMap();
    }
}
