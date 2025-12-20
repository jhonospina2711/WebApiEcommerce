using System;
using WebApiEcommerce.Model;
using WebApiEcommerce.Repository.IRepository;

namespace WebApiEcommerce.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;

    public UserRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public bool CreateUser(User user)
    {
        if (user == null)
        {
            return false;
        }
        user.CreationDate = DateTime.Now;
        user.UpdateDate = DateTime.Now;
        _db.Users.Add(user);
        return Save();
    }

    public bool DeleteUser(User user)
    {
        if (user == null)
        {
            return false;
        }
        _db.Users.Remove(user);
        return Save();
    }

    public User? GetUser(int userId)
    {
        if (userId <= 0)
        {
            return null;
        }
        return _db.Users.FirstOrDefault(u => u.Id == userId);
    }

    public User? GetUserByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }
        return _db.Users.FirstOrDefault(u => u.Email.ToLower().Trim() == email.ToLower().Trim());
    }

    public ICollection<User> GetUsers()
    {
        return _db.Users.OrderBy(u => u.Name).ToList();
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0;
    }

    public bool UpdateUser(User user)
    {
        if (user == null)
        {
            return false;
        }
        user.UpdateDate = DateTime.Now;
        _db.Users.Update(user);
        return Save();
    }

    public bool UserExists(int id)
    {
        if (id <= 0)
        {
            return false;
        }
        return _db.Users.Any(u => u.Id == id);
    }

    public bool UserExists(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }
        return _db.Users.Any(u => u.Email.ToLower().Trim() == email.ToLower().Trim());
    }
}
