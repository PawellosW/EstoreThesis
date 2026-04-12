using E_Store_Sentiment_Analysis_Thesis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<List<Order>> GetFilteredOrdersAsync(string? status, string? orderDate);

        Task<(bool Success, string ErrorMessage)> PlaceOrderAsync(int userId, List<CartItem> cart);

        Task<(bool IsValid, string Error)> CheckAvailability(List<CartItem> cart);

        Task<Order> CreateOrderAsync(int userId, List<CartItem> cart);

        Task AddOrderItemsAndUpdateStockAsync(Order order, List<CartItem> cart);
    }
}
