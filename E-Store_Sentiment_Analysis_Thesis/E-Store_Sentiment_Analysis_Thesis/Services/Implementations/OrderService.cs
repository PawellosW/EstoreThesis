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
    public class OrderService : IOrderService
    {
        private readonly StoreContext _context;

        public OrderService(StoreContext context)
        {
            _context = context;
        }


        public async Task<List<Order>> GetFilteredOrdersAsync(string? status, string? orderDate)
        {
            var query = _context.Orders
            .Include(o => o.Customer)
            .AsQueryable();

            
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            
            if (!string.IsNullOrEmpty(orderDate) && DateTime.TryParse(orderDate, out DateTime date))
            {
                query = query.Where(o => o.OrderDate.Date == date.Date);
            }

            return await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }



        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users
           .OrderBy(u => u.LastName)
           .ToListAsync();
        }

        

        public async Task<(bool IsValid, string Error)> CheckAvailability(List<CartItem> cart)
        {
            foreach (var item in cart)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                {
                    return (false, $"Produkt {item.Name} ma niewystarczającą ilość w magazynie.");
                }
            }

            return (true, null);
        }

        public async Task<Order> CreateOrderAsync(int userId, List<CartItem> cart)
        {
            var order = new Order
            {
                CustomerId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(c => c.Total),
                Status = "Pending"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }


        public async Task AddOrderItemsAndUpdateStockAsync(Order order, List<CartItem> cart)
        {
            foreach (var item in cart)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                _context.ProductsOrders.Add(new ProductsOrder
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                });

                product.Stock -= item.Quantity;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<(bool Success, string ErrorMessage)> PlaceOrderAsync(int userId, List<CartItem> cart)
        {
 
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                
                var (isValid, error) = await CheckAvailability(cart);
                if (!isValid) return (false, error);

                
                var order = await CreateOrderAsync(userId, cart);

                
                await AddOrderItemsAndUpdateStockAsync(order, cart);

                
                await transaction.CommitAsync();
                return (true, null);
            }
            catch (Exception)
            {
                
                await transaction.RollbackAsync();
                return (false, "Wystąpił błąd podczas składania zamówienia.");
            }
        }
    }
}
