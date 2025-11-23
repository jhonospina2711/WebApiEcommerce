using System;

namespace WebApiEcommerce.Repository;

public interface ICategoryRepository
{

    ICollection<Category> GetCategories();

    Category GetCategory(int categoryId);

    bool CategoryExists(int Id);

    bool CategoryExists(string name);

    bool CreateCategory(Category category);

    bool UpdateCategory(Category category);

    bool DeleteCategory(Category category);

    bool Save();

}
