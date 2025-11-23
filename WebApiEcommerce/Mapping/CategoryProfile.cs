using System;
using AutoMapper;
using WebApiEcommerce.Model.Dtos;

namespace WebApiEcommerce.Mapping;

public class CategoryProfile: Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CreateCategoryDto>().ReverseMap();
    }

}
