using System;

namespace WebApiEcommerce.Model.Dtos;

public class UpdateUserDto
{
    public string? Name { get; set;}
    public string? Username { get; set;}
    public string? Password { get; set;}
    public string? Role { get; set;}
    public bool IsActive { get; set;} = true;
    public DateTime? UpdateDate { get; set; } = null;


}
