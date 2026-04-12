using E_Store_Sentiment_Analysis_Thesis.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Interfaces
{
    public interface IProductService
    {
       
        Task<IEnumerable<Product>> GetAllProductsAsync(int? categoryId);
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();

       
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

      
        Task<bool> ProductExistsAsync(int id);
    }
}
