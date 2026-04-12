using E_Store_Sentiment_Analysis_Thesis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Interfaces
{
    public interface IStoreReviewService
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync();

        Task AddReviewAsync(Review review);
    }
}
