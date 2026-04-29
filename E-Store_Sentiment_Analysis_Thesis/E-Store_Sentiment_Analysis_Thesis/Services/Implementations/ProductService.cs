using E_Store_Sentiment_Analysis_Thesis.Data;
using E_Store_Sentiment_Analysis_Thesis.Models;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly StoreContext _context;

        public ProductService(StoreContext context)
        {
            _context = context;
        }

        public async Task AddReviewAsync(Review review)
        {
            review.CreatedAt = DateTime.Now;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(int? categoryId)
        {
            var products = _context.Products.Include(p => p.Category).AsQueryable();
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }
            return await products.ToListAsync();
        }



        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews) // Pobierz opinie
                .ThenInclude(r => r.User) // I autorów tych opinii
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            product.CreatedAt = DateTime.Now; // Logika biznesowa w serwisie!
            _context.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(e => e.Id == id);
        }
    }
}