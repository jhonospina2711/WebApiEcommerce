using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiEcommerce.Model;
using WebApiEcommerce.Model.Dtos;
using WebApiEcommerce.Repository;
using WebApiEcommerce.Repository.IRepository;

namespace WebApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
          private readonly IProductRepository _productRepository;
          private readonly ICategoryRepository _categoryRepository;

        // Servicio de AutoMapper para convertir entidades en DTOs y viceversa
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            // Guarda el repositorio de categorías recibido por inyección de dependencias
            _productRepository = productRepository;
            // Guarda el repositorio de categorías recibido por inyección de dependencias
            _categoryRepository = categoryRepository;
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

        [HttpGet("{productId:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public IActionResult GetProduct(int productId)
        {
            // Busca la categoría por su productId en la base de datos
            var product = _productRepository.GetProduct(productId);

            // Si no se encuentra la categoría, devuelve un error 404 Not Found
            if (product == null)
            {
                return NotFound($"El producto con el id: {productId} no existe.");
            }

            // Convierte la entidad Product en un DTO para enviar solo los datos necesarios
            var productDto = _mapper.Map<ProductDto>(product);

            // Devuelve la categoría encontrada en formato JSON con código 200 OK
            return Ok(productDto);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            // Verifica que el objeto recibido no sea nulo
            if (createProductDto == null)
            {
                return BadRequest(ModelState);
            }

            // Verifica si ya existe una categoría con el mismo nombre
            if (_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError", "La categoria ya existe.");
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"La categoria con el id {createProductDto.CategoryId} no existe.");
                return BadRequest(ModelState);
            }

            // Convierte el DTO recibido en una entidad Product usando AutoMapper
            var product = _mapper.Map<Product>(createProductDto);

            // Intenta guardar la nueva categoría en la base de datos
            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salio mal guardando el registro {product.Name}");
                return StatusCode(500, ModelState);
            }

            // Devuelve una respuesta 201 Created con la ruta del nuevo recurso
            return CreatedAtRoute("GetProduct", new { productId = product.ProductId }, product);
        }

    }
}
