using System;
using AutoMapper;
using WebApiEcommerce.Model;
using WebApiEcommerce.Model.Dtos;

namespace WebApiEcommerce.Mapping;

public class UserProfile:Profile
{
    public UserProfile()
    {
       CreateMap<User, UserDto>().ReverseMap(); 
       CreateMap<User, CreateUserDto>().ReverseMap(); 
       CreateMap<User, UserLoginDto>().ReverseMap(); 
       CreateMap<User, UserLoginResponseDto>().ReverseMap(); 
       CreateMap<UpdateUserDto, User>()
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateActiveUser, User>().ReverseMap();
    }

}
