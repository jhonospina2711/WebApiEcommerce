using System;
using WebApiEcommerce.Model;

namespace WebApiEcommerce.Repository.IRepository;

public interface IUserRepository
{
    ICollection<User> GetUsers();
    
    User? GetUser(int userId);
    
    bool UserExists(int Id);
    
    bool UserExists(string username);
    
    bool CreateUser(User user);
    
    bool UpdateUser(User user);
    
    bool DeleteUser(User user);
    
    bool Save();
}
