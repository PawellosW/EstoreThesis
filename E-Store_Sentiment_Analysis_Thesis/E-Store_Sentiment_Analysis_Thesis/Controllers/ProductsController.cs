using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Store_Sentiment_Analysis_Thesis.Data;
using E_Store_Sentiment_Analysis_Thesis.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;


namespace E_Store_Sentiment_Analysis_Thesis.Controllers
{






    public class ProductsController : Controller
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        // LISTA PRODUKTÓW


        public async Task<IActionResult> Index(int? categoryId)
        {
            ViewBag.Categories = await _service.GetAllCategoriesAsync();
            ViewData["SelectedCategoryId"] = categoryId;

            var products = await _service.GetAllProductsAsync(categoryId);
            return View(products);
        }

        // CREATE (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PrepareCategoriesViewBag();
            return View();
        }

        // CREATE (POST)
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

        // EDIT (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _service.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();

            await PrepareCategoriesViewBag();
            return View(product);
        }

        // EDIT (POST)
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

        // DELETE (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _service.GetProductByIdAsync(id.Value);
            return product == null ? NotFound() : View(product);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POMOCNIK UI (Zostaje w kontrolerze, bo dotyczy widoku)
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
    }
}