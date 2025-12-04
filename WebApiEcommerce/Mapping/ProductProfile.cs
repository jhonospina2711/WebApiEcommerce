using System;
using WebApiEcommerce.Model;
using WebApiEcommerce.Model.Dtos;
using AutoMapper;

namespace WebApiEcommerce.Mapping;

public class ProductProfile: Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap(); 
        CreateMap<Product, CreateProductDto>().ReverseMap(); 
        CreateMap<Product, UpdateProductDto>().ReverseMap(); 
        
    }


}
