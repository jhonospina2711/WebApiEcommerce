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

        //! Servicio de AutoMapper para convertir entidades en DTOs y viceversa
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

        //! Obtener todos los productos
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

        //! Obtener un producto por su Id
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

        //! Crear un nuevo producto
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

            // verificar si existe el producto con el mismo nombre
            if (_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError", "El Producto ya existe.");
                return BadRequest(ModelState);
            }

            // Verifica si la categoría asociada existe
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

            var createdProduct = _productRepository.GetProduct(product.ProductId);
            var productDto = _mapper.Map<ProductDto>(createdProduct);
            // Devuelve una respuesta 201 Created con la ruta del nuevo recurso
            return CreatedAtRoute("GetProduct", new { productId = product.ProductId },  productDto);
        }


        //! Obtener productos por categoría
        [HttpGet("searchProductByCategory/{categoryId:int}", Name = "GetProductsForCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductsForCategory(int categoryId)
        {
            // Busca la categoría por su categoryId en la base de datos
            var products = _productRepository.GetProductsForCategory(categoryId);

            // Si no se encuentra la categoría, devuelve un error 404 Not Found
            if (products.Count == 0)
            {
                return NotFound($"No existen productos asociados a la categoria id: {categoryId}.");
            }

            // Convierte la entidad Product en un DTO para enviar solo los datos necesarios
            var productsDto = _mapper.Map<List<ProductDto>>(products);

            // Devuelve la categoría encontrada en formato JSON con código 200 OK
            return Ok(productsDto);
        }

        //! Buscar productos por nombre o descripción
        [HttpGet("searchProductByNameOrDescription/{searchTerm}", Name = "SearchProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult SearchProducts(string searchTerm)
        {
            // Busca la categoría por su categoryId en la base de datos
            var products = _productRepository.SearchProducts(searchTerm);

            // Si no se encuentra la categoría, devuelve un error 404 Not Found
            if (products.Count == 0)
            {
                return NotFound($"Los productos con el nombre o descripción '{searchTerm}' No existen.");
            }

            // Convierte la entidad Product en un DTO para enviar solo los datos necesarios
            var productsDto = _mapper.Map<List<ProductDto>>(products);

            // Devuelve la categoría encontrada en formato JSON con código 200 OK
            return Ok(productsDto);
        }
        

        //! Comprar un producto por nombre y cantidad
        [HttpPatch("buyProduct/{name}/{quantity:int}", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult BuyProduct(string name, int quantity)
        {
            //var products = _productRepository.BuyProducts(name,quantity);
            if(string.IsNullOrWhiteSpace(name) || quantity <= 0)
            {
                return BadRequest("El nombre del producto o la cantidad no son validos.");
            }

            var foundProduct = _productRepository.ProductExists(name);
            if (!foundProduct)
            {
                return NotFound($"El producto con el nombre {name} no existe");
            }
            if (!_productRepository.BuyProduct(name, quantity))
            {
                ModelState.AddModelError("CustomError", $"No se pudo completar la compra del producto '{name}' o la cantidad {quantity} solicitada es mayor al stock disponible.");
                return BadRequest(ModelState);
            }
            //Se compro 1 unidadad del producto '{name}'
            //Se compro 2 unidadades del producto '{name}'
            var units = quantity == 1 ? "unidad" : "unidades";
            return Ok($"Se compro {quantity} {units} del producto '{name}'.");
           
        }

        //! Actualizar un producto existente
        [HttpPut("{productId:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public IActionResult UpdateProduct(int productId, [FromBody] UpdateProductDto updateProductDto)
        {
            // Verifica que el objeto recibido no sea nulo
            if (updateProductDto == null)
            {
                return BadRequest(ModelState);
            }

            // Verifica si ya existe una categoría con el mismo nombre
            if (!_productRepository.ProductExists(productId))
            {
                ModelState.AddModelError("CustomError", "El producto no existe.");
                return BadRequest(ModelState);
            }

            // Verifica si la categoría asociada existe
            if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"La categoria con el id {updateProductDto.CategoryId} no existe.");
                return BadRequest(ModelState);
            }

            // Convierte el DTO recibido en una entidad Product usando AutoMapper
            var product = _mapper.Map<Product>(updateProductDto);
            product.ProductId = productId;

            // Intenta guardar la nueva categoría en la base de datos
            if (!_productRepository.UpdateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salio mal al actualizar el registro {product.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    
        [HttpDelete("{productId:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteProduct(int productId)
        {
            if(productId == 0)
            {
                return BadRequest();
            }
            //! valida si el producto existe
            var product = _productRepository.GetProduct(productId);

            //! Si no se encuentra la categoría, devuelve un error 404 Not Found
            if (product == null)
            {
                return NotFound($"El producto con el id: {productId} no existe.");
            }

            if (!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salio mal al eliminar el registro {product.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    
    }
}
