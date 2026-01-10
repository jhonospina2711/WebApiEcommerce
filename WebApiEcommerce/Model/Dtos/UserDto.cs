using System;

namespace WebApiEcommerce.Model.Dtos;

public class UserDto
{
    public int Id {get; set;}
    public string? Name { get; set;}
    public string? Username { get; set;}
    public string? Password { get; set;}
    public string? Role { get; set;}
    public bool IsActive { get; set;} = false;
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public DateTime? UpdateDate { get; set; } = null;

}
