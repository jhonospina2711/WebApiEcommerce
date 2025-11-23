using System;
using System.ComponentModel.DataAnnotations;

namespace WebApiEcommerce.Model.Dtos;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
    [MinLength(3, ErrorMessage = "El nombre debe tener al menos 3 caracteres")]
    public string Name { get; set; } = string.Empty;
}
