using System;

namespace WebApiEcommerce.Model;

public class User
{
    public int Id {get; set;}
    public string? Name { get; set;}
    public string Username { get; set;} = string.Empty;
    public string? Password { get; set;}
    public string? Role { get; set;}
    public bool IsActive { get; set;} = true;
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public DateTime? UpdateDate { get; set; } = null;

}
