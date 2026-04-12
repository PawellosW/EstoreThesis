using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Store_Sentiment_Analysis_Thesis.Data;
using E_Store_Sentiment_Analysis_Thesis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;

namespace E_Store_Sentiment_Analysis_Thesis.Controllers
{
    public class StoreReviewController : Controller
    {
        private readonly IStoreReviewService _service;

        public StoreReviewController(IStoreReviewService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var reviews = await _service.GetAllReviewsAsync();
            return View(reviews);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddStoreReview(string text)
        {
            if ( string.IsNullOrWhiteSpace(text) ) ModelState.AddModelError("Text", "Opinia nie moze być pusta");
            
            if (!ModelState.IsValid)
            {
                var reviews = await _service.GetAllReviewsAsync();
                return View("Index", reviews);
            }

          

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
           
            var review = new Review
            {
                UserId = userId,
                Text = text,
                ProductId = null
            };

            await _service.AddReviewAsync(review);
            
            return RedirectToAction(nameof(Index));
        }
    }
}
