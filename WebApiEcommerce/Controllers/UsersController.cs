using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiEcommerce.Model;
using WebApiEcommerce.Model.Dtos;
using WebApiEcommerce.Repository.IRepository;

namespace WebApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        // Servicio de AutoMapper para convertir entidades en DTOs y viceversa
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper )
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        //! Obtener todos los usuarios
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public IActionResult GetUsers()
        {
            var users = _userRepository.GetUsers();
            var usersDto = _mapper.Map<List<UserDto>>(users);
            return Ok(usersDto);
        }

        //! Obtener un usuarios por su Id
        [HttpGet("{id:int}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public IActionResult GetUser(int id)
        {
            var user = _userRepository.GetUser(id);
            if(user == null)
            {
                return NotFound($"El usuario con Id {id} no existe.");
            }
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        //! Registrar un nuevo usuario
        [HttpPost(Name = "RegisterUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
        {
            // validar el objeto recibido
            if (createUserDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // verificar si el usuario es null o está vacío
            if (string.IsNullOrWhiteSpace(createUserDto.Username))
            {
                return BadRequest("Username es requerido");
            }

            // verificar si el usuario ya existe
            if (!_userRepository.IsUniqueUser(createUserDto.Username))
            {
                return BadRequest("El usuario ya existe");
            }

            // registrar el usuario
            var result = await _userRepository.Register(createUserDto);
            
            // si el resultado es null, retornar error 500
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al registrar el usuario");
            }
            return CreatedAtRoute("GetUser", new { id = result.Id}, result);
        }

        //! Registrar un nuevo usuario
        [HttpPost("Login", Name = "LoginUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> RegisterUser([FromBody] UserLoginDto userLoginDto)
        {
            // validar el objeto recibido
            if (userLoginDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // registrar el usuario
            var user = await _userRepository.Login(userLoginDto);
            
            // si el resultado es null, retornar error 500
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }

        //! Buscar usuarios por nombre o descripción
        // [HttpGet("searchUsersByNameOrDescription/{searchTerm}", Name = "SearchUser")]
        // [ProducesResponseType(StatusCodes.Status403Forbidden)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(StatusCodes.Status200OK)]


        //! Inactivar un usuario por su Id
        [HttpPut("{userId:int}", Name = "UpdatedActiveUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 

        public IActionResult UpdatedActiveUser(int userId, [FromBody] UpdateActiveUser updateActiveUser)
        {
            // Verifica que el objeto recibido no sea nulo
            if (updateActiveUser == null)
            {
                return BadRequest(ModelState);
            }

            // Verifica si ya existe un usuario con el id dado
            var userDb = _userRepository.GetUser(userId);
            if (userDb == null)
            {
                ModelState.AddModelError("CustomError", "El usuario no existe.");
                return BadRequest(ModelState);
            }

            // Actualiza solo los campos necesarios
            _mapper.Map(updateActiveUser, userDb); // Mapea los campos del DTO sobre la entidad existente

            // Intenta actualizar el IsActive del usuario (o cualquier otro campo relevante)
            if (!_userRepository.updatedActiveUser(userDb))
            {
                ModelState.AddModelError("CustomError", $"Algo salio mal al actualizar el usuario {userDb.Username}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }
}
