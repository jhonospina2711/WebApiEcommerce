using System;

namespace WebApiEcommerce.Model.Dtos;

public class UserDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; }
    
    public string? Address { get; set; }
    
    public DateTime CreationDate { get; set; }
}
