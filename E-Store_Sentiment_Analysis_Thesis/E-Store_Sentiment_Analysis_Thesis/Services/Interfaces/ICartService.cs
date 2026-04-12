using E_Store_Sentiment_Analysis_Thesis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Interfaces
{
   public interface ICartService
    {
        List<CartItem> GetCart();
        void SaveCart(List<CartItem> cart);
        void ClearCart();



        Task<bool> AddToCartAsync(int productId, int quantity);
        Task UpdateQuantityAsync(int productId, int quantity);
        void RemoveFromCart(int productId);


    }
}
