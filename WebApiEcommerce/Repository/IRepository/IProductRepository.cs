using System;
using WebApiEcommerce.Model;

namespace WebApiEcommerce.Repository.IRepository;

public interface IProductRepository 
{
    ICollection<Product> GetProducts();
    ICollection<Product> GetProductsForCategory(int categoryId);
    ICollection<Product> SearchProduct(string name);
    Product? GetProduct(int productId);
    bool BuyProduct(string Name, int quantity);
    bool ProductExists(int Id);
    bool ProductExists(string Name);
    bool CreateProduct(Product product);
    bool UpdateProduct(Product product);
    bool DeleteProduct(Product product);
    bool Save();


}
