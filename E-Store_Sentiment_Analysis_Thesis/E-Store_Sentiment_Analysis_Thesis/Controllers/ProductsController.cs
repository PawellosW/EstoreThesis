using E_Store_Sentiment_Analysis_Thesis.Models;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_Store_Sentiment_Analysis_Thesis.Controllers
{


    public class ProductsController : Controller
    {
        private readonly IProductService _service;
        private readonly ISentimentParserService _parser;

        public ProductsController(IProductService service, ISentimentParserService parserService)
        {
            _service = service;
            _parser = parserService;
        }


        public async Task<IActionResult> Index(int? categoryId)
        {
            ViewBag.Categories = await _service.GetAllCategoriesAsync();
            ViewData["SelectedCategoryId"] = categoryId;

            var products = await _service.GetAllProductsAsync(categoryId);
            return View(products);
        }

      
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PrepareCategoriesViewBag();
            return View();
        }

     
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _service.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            await PrepareCategoriesViewBag();
            return View(product);
        }

     
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _service.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();

            await PrepareCategoriesViewBag();
            return View(product);
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try { await _service.UpdateProductAsync(product); }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _service.ProductExistsAsync(product.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await PrepareCategoriesViewBag();
            return View(product);
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _service.GetProductByIdAsync(id.Value);
            return product == null ? NotFound() : View(product);
        }

        
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

       
        private async Task PrepareCategoriesViewBag()
        {
            var categories = await _service.GetAllCategoriesAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _service.GetProductByIdAsync(id.Value);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddProductReview(int productId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                // Jeśli pusta opinia, po prostu wróć do szczegółów
                return RedirectToAction(nameof(Details), new { id = productId });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var review = new Review
            {
                UserId = userId,
                ProductId = productId,
                Text = text,
                CreatedAt = DateTime.Now
            };

            await _service.AddReviewAsync(review);

            return RedirectToAction(nameof(Details), new { id = productId });
        }


    }
}