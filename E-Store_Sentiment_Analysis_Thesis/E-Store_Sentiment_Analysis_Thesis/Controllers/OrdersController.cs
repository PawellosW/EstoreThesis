using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_Store_Sentiment_Analysis_Thesis.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrdersController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? status, string? orderDate)
        {
            
            ViewBag.Customers = await _orderService.GetUsersAsync();
            var orders = await _orderService.GetFilteredOrdersAsync(status, orderDate);

            return View(orders);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmCheckout()
        {
           
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId) || userId == 0)
            {
                return Unauthorized();
            }

           
            var cart = _cartService.GetCart();

            if (!cart.Any())

            {
                return View("Checkout", cart);
            }

            
            var (success, errorMessage) = await _orderService.PlaceOrderAsync(userId, cart);

            if (!success)
            {
                
                ModelState.AddModelError("", errorMessage);
                return View("~/Views/Cart/Checkout.cshtml", cart);
            }

            
            _cartService.ClearCart();
            return RedirectToAction("OrderConfirmation");
        }


        public IActionResult OrderConfirmation()
        {
            return View();
        }

    }
}
