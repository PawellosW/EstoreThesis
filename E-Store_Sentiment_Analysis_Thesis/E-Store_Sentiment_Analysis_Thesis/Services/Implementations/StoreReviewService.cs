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
    public class StoreReviewService : IStoreReviewService
    {
        private readonly StoreContext _context;

        public StoreReviewService(StoreContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            var reviews = _context.Reviews
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Where(r => r.ProductId == null);

            
            return await reviews.ToListAsync();
        }


        public async Task AddReviewAsync(Review review)
        {
            review.CreatedAt = DateTime.Now; 
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

    }
}
