using E_Store_Sentiment_Analysis_Thesis.Models;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

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
