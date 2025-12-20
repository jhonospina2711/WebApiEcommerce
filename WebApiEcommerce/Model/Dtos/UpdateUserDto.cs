using System.ComponentModel.DataAnnotations;

namespace WebApiEcommerce.Model.Dtos;

public class UpdateUserDto
{
    [Required(ErrorMessage = "El ID es obligatorio")]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    [MinLength(3, ErrorMessage = "El nombre debe tener al menos 3 caracteres")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    public string Email { get; set; } = string.Empty;
    
    [Phone(ErrorMessage = "El número de teléfono no es válido")]
    public string? PhoneNumber { get; set; }
    
    [MaxLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
    public string? Address { get; set; }
}
