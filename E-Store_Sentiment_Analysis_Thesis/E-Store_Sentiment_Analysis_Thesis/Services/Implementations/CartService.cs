using E_Store_Sentiment_Analysis_Thesis.Data;
using E_Store_Sentiment_Analysis_Thesis.Models;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly StoreContext _context;
        private const string CartKey = "Cart";

        public CartService(IHttpContextAccessor httpContext, StoreContext context)
        {
            _httpContext = httpContext;
            _context = context;
        }




        public void ClearCart()
        {
            _httpContext.HttpContext?.Session.Remove(CartKey);
        }


        public List<CartItem> GetCart()
        {
            var cartJson = _httpContext.HttpContext?.Session.GetString(CartKey);
            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }


        public void SaveCart(List<CartItem> cart)
        {
            _httpContext.HttpContext?.Session.SetString(CartKey, JsonSerializer.Serialize(cart));
        }

        public async Task<bool> AddToCartAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.Stock < quantity)
                return false;

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += (quantity <= 0 ? 1 : quantity);
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price ?? 0,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
            return true;
        }

        public async Task UpdateQuantityAsync(int productId, int quantity)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem == null) return;

            if (quantity <= 0)
            {
                cart.Remove(cartItem);
            }
            else
            {
                var product = await _context.Products.FindAsync(productId);
                if (product != null && quantity > product.Stock)
                {
                    cartItem.Quantity = product.Stock ?? 0;
                }
                else
                {
                    cartItem.Quantity = quantity;
                }
            }

            SaveCart(cart);
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCart(cart);
            }
        }
    }
    }

