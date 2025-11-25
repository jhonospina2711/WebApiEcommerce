using System;

namespace WebApiEcommerce.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    public bool CategoryExists(int Id)
    {
        // Verifica si existe una categoría con el Id especificado
        return _db.Categories.Any(c=> c.Id == Id);
    }

    public bool CategoryExists(string name)
    {
        // Verifica si ya existe una categoría con el nombre dado (ignorando mayúsculas/minúsculas y espacios)
        return _db.Categories.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool CreateCategory(Category category)
    {
        // Crea una nueva categoría: asigna la fecha actual, la agrega a la base de datos y guarda los cambios
        category.CreationDate = DateTime.Now;
        _db.Categories.Add(category);
        return Save();
    }

    public bool DeleteCategory(Category category)
    {
        // Elimina una categoría de la base de datos y guarda los cambios
        _db.Categories.Remove(category);
        return Save();
    }

    public ICollection<Category> GetCategories()
    {
        // Obtiene todas las categorías ordenadas por nombre
        return _db.Categories.OrderBy(c=> c.Name).ToList();
    }

    public Category? GetCategory(int id)
    {
        // Busca y retorna la categoría con el Id dado. Si no existe, lanza una excepción.
        return _db.Categories.FirstOrDefault(c => c.Id == id);
    }

    public bool Save()
    {
        // Guarda los cambios realizados en la base de datos
        return _db.SaveChanges() >= 0 ? true:false;   
    }

    public bool UpdateCategory(Category category)
    {
        // Actualiza los datos de una categoría existente y guarda los cambios
        category.CreationDate = DateTime.Now;
        _db.Categories.Update(category);
        return Save();
    }
}
