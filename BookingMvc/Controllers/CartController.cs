using BookingData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingMvc.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IEventRepository repository;

        public CartController(IEventRepository repo)
        {
            repository = repo;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<Cart>("cart") ?? new Cart();
            var events = repository.Events
                .Where(e => cart.EventIds.Contains((int)e.EventId!.Value));
            return View(events);
        }

        public IActionResult Add(int eventId)
        {
            var cart = HttpContext.Session.GetObject<Cart>("cart") ?? new Cart();
            cart.AddItem(eventId);
            HttpContext.Session.SetObject("cart", cart);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Remove(int eventId)
        {
            var cart = HttpContext.Session.GetObject<Cart>("cart") ?? new Cart();
            cart.RemoveItem(eventId);
            HttpContext.Session.SetObject("cart", cart);
            return RedirectToAction("Index");
        }
    }
}
