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
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsers()
        {
            var users = _userRepository.GetUsers();
            var usersDto = new List<UserDto>();

            foreach (var user in users)
            {
                usersDto.Add(_mapper.Map<UserDto>(user));
            }
            return Ok(usersDto);
        }

        [HttpGet("{id:int}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUser(int id)
        {
            var user = _userRepository.GetUser(id);

            if (user == null)
            {
                return NotFound($"El usuario con el id {id} no existe.");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpGet("email/{email}", Name = "GetUserByEmail")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUserByEmail(string email)
        {
            var user = _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                return NotFound($"El usuario con el email {email} no existe.");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_userRepository.UserExists(createUserDto.Email))
            {
                ModelState.AddModelError("CustomError", "El usuario con este email ya existe.");
                return BadRequest(ModelState);
            }

            var user = _mapper.Map<User>(createUserDto);

            if (!_userRepository.CreateUser(user))
            {
                ModelState.AddModelError("CustomError", $"Algo salio mal guardando el registro {user.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetUser", new { id = user.Id }, user);
        }

        [HttpPatch("{id:int}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!_userRepository.UserExists(id))
            {
                return NotFound($"El usuario con el id {id} no existe.");
            }

            if (updateUserDto == null || updateUserDto.Id != id)
            {
                return BadRequest(ModelState);
            }

            // Verificar si el email ya existe en otro usuario
            var existingUser = _userRepository.GetUserByEmail(updateUserDto.Email);
            if (existingUser != null && existingUser.Id != id)
            {
                ModelState.AddModelError("CustomError", "El email ya está en uso por otro usuario.");
                return BadRequest(ModelState);
            }

            var user = _mapper.Map<User>(updateUserDto);

            if (!_userRepository.UpdateUser(user))
            {
                ModelState.AddModelError("CustomError", $"Algo salio mal actualizando el registro {user.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteUser(int id)
        {
            if (!_userRepository.UserExists(id))
            {
                return NotFound($"El usuario con el id {id} no existe.");
            }

            var user = _userRepository.GetUser(id);

            if (user == null)
            {
                return NotFound($"El usuario con el id {id} no existe.");
            }

            if (!_userRepository.DeleteUser(user))
            {
                ModelState.AddModelError("CustomError", $"Algo salio mal eliminando el registro {user.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
