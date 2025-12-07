using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiEcommerce.Repository.IRepository;

namespace WebApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
          private readonly IProductRepository _productRepository;

        // Servicio de AutoMapper para convertir entidades en DTOs y viceversa
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, IMapper mapper)
        {
            // Guarda el repositorio de categorías recibido por inyección de dependencias
            _productRepository = productRepository;
            // Guarda el servicio de AutoMapper recibido por inyección de dependencias
            _mapper = mapper;
        }

    }
}
