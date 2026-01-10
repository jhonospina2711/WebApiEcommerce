using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApiEcommerce.Model;
using WebApiEcommerce.Model.Dtos;
using WebApiEcommerce.Repository.IRepository;

namespace WebApiEcommerce.Repository;



public class UserRepository : IUserRepository
{

    public readonly ApplicationDbContext _db;
    private string? secretKey;

    public UserRepository(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
    }
    
    public User? GetUser(int id)
    {
        return _db.Users.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<User> GetUsers()
    {
        return _db.Users.OrderBy(u => u.Username).ToList();
    }

    public bool IsUniqueUser(string username)
    {
        //Si encuentra un usuario con ese nombre, devuelve false, si no lo encuentra devuelve true
        return !_db.Users.Any(u => u.Username.ToLower().Trim() == username.ToLower().Trim());
    }

    public async Task<User> Register(CreateUserDto createUserDto)
    {
        var encriptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        var user = new User()
        {
            Username = createUserDto.Username ?? "No Username",
            Name = createUserDto.Name,
            Role = createUserDto.Role,
            Password = encriptedPassword,
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.Username))
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "El usuario o la contraseña son incorrectos - User requerido"
            };
        }
        var user = _db.Users.FirstOrDefault<User>(u => u.Username.ToLower().Trim() == userLoginDto.Username.ToLower());
        if (user == null)
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "El usuario o la contraseña son incorrectos - User no encontrado"
            };

        }
        if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "El usuario o la contraseña son incorrectos - password incorrecto"
            };
        }
        //Generar el token JWT
        var handlerToken = new JwtSecurityTokenHandler();
        if (string.IsNullOrEmpty("SecretKey No esta configurada"))
        {
            throw new Exception("SecretKey No esta configurada");
        }
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
            }

            ),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = handlerToken.CreateToken(tokenDescriptor);
        return new UserLoginResponseDto()
        {
            Token = handlerToken.WriteToken(token),
            User = new UserRegisterDto()
            {
                Username = user.Username,
                Name = user.Name,
                Role = user.Role,
                Password = user.Password ?? ""

            },

            Message = "Usuario logueado con exito",
        };

    }

    public bool UserExists(int Id)
    {
        if (Id <= 0)
        {
            return false;
        }
        return _db.Users.Any(u => u.Id == Id);
    }

    public bool UserExists(string name)
    {
         if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }
        return _db.Users.Any(u => u.Username.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool updatedActiveUser(User user)
    {
        if (user == null)
        {
            return false;
        }
        user.UpdateDate = DateTime.Now;
        _db.Users.Update(user);
        return Save();
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0;
    }

}
