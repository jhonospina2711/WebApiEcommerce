using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiEcommerce.Model.Dtos;
using WebApiEcommerce.Repository;

namespace WebApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        // Repositorio para acceder y manejar los datos de las categorías
        private readonly ICategoryRepository _categoryRepository;

        // Servicio de AutoMapper para convertir entidades en DTOs y viceversa
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            // Guarda el repositorio de categorías recibido por inyección de dependencias
            _categoryRepository = categoryRepository;
            // Guarda el servicio de AutoMapper recibido por inyección de dependencias
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public IActionResult GetCategories()
        {
            // Obtiene todas las categorías desde el repositorio
            var categories = _categoryRepository.GetCategories();

            // Crea una lista para almacenar los DTOs de las categorías
            var categoriesDto = new List<CategoryDto>();

            // Convierte cada entidad Category en un DTO usando AutoMapper
            foreach (var category in categories)
            {
                categoriesDto.Add(_mapper.Map<CategoryDto>(category));
            }
            return Ok(categoriesDto);
        }


        [HttpGet("{id:int}", Name ="GetCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public IActionResult GetCategory(int id)
        {
            // Busca la categoría por su id en la base de datos
            var category = _categoryRepository.GetCategory(id);

            // Si no se encuentra la categoría, devuelve un error 404 Not Found
            if (category == null)
            {
                return NotFound($"La categoria con el id {id} no existe.");
            }

            // Convierte la entidad Category en un DTO para enviar solo los datos necesarios
            var categoryDto = _mapper.Map<CategoryDto>(category);

            // Devuelve la categoría encontrada en formato JSON con código 200 OK
            return Ok(categoryDto);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            // Verifica que el objeto recibido no sea nulo
            if (createCategoryDto == null)
            {
                return BadRequest(ModelState);
            }

            // Verifica si ya existe una categoría con el mismo nombre
            if (_categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "La categoria ya existe.");
                return BadRequest(ModelState);
            }

            // Convierte el DTO recibido en una entidad Category usando AutoMapper
            var category = _mapper.Map<Category>(createCategoryDto);

            // Intenta guardar la nueva categoría en la base de datos
            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salio mal guardando el registro {category.Name}");
                return StatusCode(500, ModelState);
            }

            // Devuelve una respuesta 201 Created con la ruta del nuevo recurso
            return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
        }

        [HttpPatch("{id:int}", Name ="UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


        public IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto)
        {
            // Verifica si la categoría con el Id dado existe
            if(!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"La categoria con el id {id} no existe.");
            }
            // Verifica que el objeto recibido no sea nulo
            if (updateCategoryDto == null)
            {
                return BadRequest(ModelState);
            }

            // Verifica si ya existe una categoría con el mismo nombre
            if (_categoryRepository.CategoryExists(updateCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "La categoria ya existe.");
                return BadRequest(ModelState);
            }

            // Convierte el DTO recibido en una entidad Category usando AutoMapper
            var category = _mapper.Map<Category>(updateCategoryDto);
            category.Id = id;

            // Intenta guardar la nueva categoría en la base de datos
            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salio mal actualizando el registro {category.Name}");
                return StatusCode(500, ModelState);
            }

            // Devuelve una respuesta 204 No Content indicando que la actualización fue exitosa
            return NoContent();
        }



        [HttpDelete("{id:int}", Name ="DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


        public IActionResult DeleteCategory(int id)
        {
            // Verifica si la categoría con el Id dado existe
            if(!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"La categoria con el id {id} no existe.");
            }
           // Obtiene la categoría a eliminar
           var category = _categoryRepository.GetCategory(id);

           //verifica si la categoría es nula
           if(category == null )
            {
                return NotFound($"La categoria con el id {id} no existe.");
            }

            // Intenta eliminar la categoría de la base de datos
            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salio mal eliminando el registro {category.Name}");
                return StatusCode(500, ModelState);
            }

            
            return NoContent();
        }
    }
}
