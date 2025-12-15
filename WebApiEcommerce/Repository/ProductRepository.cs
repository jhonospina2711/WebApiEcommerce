using System;
using Microsoft.EntityFrameworkCore;
using WebApiEcommerce.Model;
using WebApiEcommerce.Repository.IRepository;

namespace WebApiEcommerce.Repository;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _db;
    
    //Este constructor inyecta el contexto de la base de datos
    public ProductRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    public bool BuyProduct(string name, int quantity)
    {
        //validamos que el nombre no sea nulo o vacio
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        //Obtenemos el producto de la base de datos por su nombre, le quitamos espacios y lo pasamos a minusculas
        var product = _db.Products.FirstOrDefault(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
        //Validamos que el producto exista y que tenga stock suficiente
        if(product == null || product.Stock < quantity)
        {
            return false;
        }
        //Disminuimos el stock del producto 
        product.Stock -= quantity;
        //Actualizamos el producto en la base de datos
        _db.Products.Update(product);
        //Guardamos los cambios en la base de datos
        return Save(); 
    }

    public bool CreateProduct(Product product)
    {
        if (product == null)
        {
            return false;
        }
        product.CreationDate = DateTime.Now;
        product.UpdateDate = DateTime.Now;
        _db.Products.Add(product);
        return Save();
    }

    public bool DeleteProduct(Product product)
    {
        if (product == null)
        {
            return false;
        }
        _db.Products.Remove(product);
        return Save();
    }

    public Product? GetProduct(int id)
    {
        if (id <= 0)
        {
            return null;
        }
        //Se busca el producto en base de datos por su Id
        return _db.Products.Include(p=> p.Category).FirstOrDefault(p => p.ProductId == id );
    }

    public ICollection<Product> GetProducts()
    {
        return _db.Products.Include(p => p.Category).OrderBy(p => p.Name).ToList();
    }

    public ICollection<Product> GetProductsForCategory(int categoryId)
    {
        if (categoryId <= 0)
        {
            return new List<Product>();
        }
        return _db.Products.Include(p => p.Category).Where( p => p.CategoryId == categoryId).OrderBy(P => P.Name).ToList();
    }

    public bool ProductExists(int Id)
    {
        if (Id <= 0)
        {
            return false;
        }
        return _db.Products.Any(p => p.ProductId == Id);
    }

    public bool ProductExists(string name)
    {
         if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }
        return _db.Products.Any(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0;
    }

    public ICollection<Product> SearchProducts(string searchTerm)
    {
        var searchTermLowered = searchTerm.ToLower().Trim();
        IQueryable<Product> queary = _db.Products;
        if(!string.IsNullOrEmpty(searchTerm))
        {
            queary = queary.Include(p => p.Category)
            .Where( p => p.Name.ToLower().Trim().Contains(searchTermLowered) || 
            p.Description.ToLower().Trim().Contains(searchTermLowered));
        }
        return queary.OrderBy(p => p.Name).ToList();
    }

    public bool UpdateProduct(Product product)
    {
        if (product == null)
        {
            return false;
        }
        product.UpdateDate = DateTime.Now;
        _db.Products.Update(product);
        return Save();
    }
}
