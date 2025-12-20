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

    public bool UserExists(int Id)
    {
        // Verifica si existe un usuario con el Id especificado
        return _db.Users.Any(u => u.Id == Id);
    }

    public bool UserExists(string username)
    {
        // Verifica si ya existe un usuario con el username dado (ignorando mayúsculas/minúsculas y espacios)
        if (string.IsNullOrWhiteSpace(username))
        {
            return false;
        }
        return _db.Users.Any(u => u.Username != null && u.Username.ToLower().Trim() == username.ToLower().Trim());
    }

    public bool CreateUser(User user)
    {
        // Crea un nuevo usuario, lo agrega a la base de datos y guarda los cambios
        if (user == null)
        {
            return false;
        }
        _db.Users.Add(user);
        return Save();
    }

    public bool DeleteUser(User user)
    {
        // Elimina un usuario de la base de datos y guarda los cambios
        if (user == null)
        {
            return false;
        }
        _db.Users.Remove(user);
        return Save();
    }

    public ICollection<User> GetUsers()
    {
        // Obtiene todos los usuarios ordenados por username
        return _db.Users.OrderBy(u => u.Username).ToList();
    }

    public User? GetUser(int id)
    {
        // Busca y retorna el usuario con el Id dado
        return _db.Users.FirstOrDefault(u => u.Id == id);
    }

    public bool Save()
    {
        // Guarda los cambios realizados en la base de datos
        return _db.SaveChanges() >= 0;
    }

    public bool UpdateUser(User user)
    {
        // Actualiza los datos de un usuario existente y guarda los cambios
        if (user == null)
        {
            return false;
        }
        _db.Users.Update(user);
        return Save();
    }
}
