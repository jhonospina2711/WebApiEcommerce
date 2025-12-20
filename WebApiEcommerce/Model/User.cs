using System.ComponentModel.DataAnnotations;

namespace WebApiEcommerce.Model;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; }
    
    public string? Address { get; set; }
    
    public DateTime CreationDate { get; set; } = DateTime.Now;
    
    public DateTime? UpdateDate { get; set; }
}
