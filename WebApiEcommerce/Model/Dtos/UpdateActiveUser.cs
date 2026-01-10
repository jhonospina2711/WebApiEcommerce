using System;

namespace WebApiEcommerce.Model.Dtos;

public class UpdateActiveUser
{
    public bool IsActive { get; set;} = false;
    public DateTime? UpdateDate { get; set; } = null;

}
