using System;
using WebApiEcommerce.Model;
using WebApiEcommerce.Model.Dtos;

namespace WebApiEcommerce.Repository.IRepository;

public interface IUserRepository
{

    ICollection<User> GetUsers();

    User? GetUser(int id);

    bool IsUniqueUser(string username);

    Task <UserLoginResponseDto> Login(UserLoginDto userLoginDto);

    Task<User> Register(CreateUserDto createUserDto);

    bool UserExists(int Id);
    bool UserExists(string name);
    bool updatedActiveUser(User user);
    bool Save();

    

}
