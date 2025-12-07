using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiEcommerce.Model.Dtos;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public IActionResult GetProducts()
        {
            // Crea una lista para almacenar los DTOs de los productos
            var products = _productRepository.GetProducts();
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            
            return Ok(productsDto);
        }

    }
}
