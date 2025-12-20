using System;
using WebApiEcommerce.Model;

namespace WebApiEcommerce.Repository.IRepository;

public interface IUserRepository
{
    ICollection<User> GetUsers();
    
    User? GetUser(int userId);
    
    User? GetUserByEmail(string email);
    
    bool UserExists(int id);
    
    bool UserExists(string email);
    
    bool CreateUser(User user);
    
    bool UpdateUser(User user);
    
    bool DeleteUser(User user);
    
    bool Save();
}
