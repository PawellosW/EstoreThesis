using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Store_Sentiment_Analysis_Thesis.Data;
using E_Store_Sentiment_Analysis_Thesis.Models;
using System.Collections.Generic;
using System.Linq;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using System;

namespace E_Store_Sentiment_Analysis_Thesis.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            await _cartService.UpdateQuantityAsync(productId, quantity);
            return RedirectToAction("Index");
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var success = await _cartService.AddToCartAsync(productId, quantity);
            if (!success)
            {
                return NotFound(); // Lub zwróć informację o braku na stanie
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public IActionResult RemoveFromCart(int productId)
        {
            _cartService.RemoveFromCart(productId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Checkout()
        {
            var cart = _cartService.GetCart();
            return View(cart);
        }

     
    }
}