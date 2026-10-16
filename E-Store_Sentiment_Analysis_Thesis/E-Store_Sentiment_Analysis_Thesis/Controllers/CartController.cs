using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
                return NotFound();
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