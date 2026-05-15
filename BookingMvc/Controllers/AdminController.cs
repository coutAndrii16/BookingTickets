using BookingData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IEventRepository repository;

        public AdminController(IEventRepository repo)
        {
            repository = repo;
        }

        public IActionResult Index() =>
            View(repository.Events.OrderBy(e => e.Date));

        public IActionResult Details(long id)
        {
            var ev = repository.Events.FirstOrDefault(e => e.EventId == id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        public IActionResult Create() =>
            View(new Event { Date = DateTime.Now.AddDays(1) });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Event ev)
        {
            if (ModelState.IsValid)
            {
                repository.CreateEvent(ev);
                TempData["message"] = $"Подію \"{ev.Title}\" успішно створено.";
                return RedirectToAction(nameof(Index));
            }
            return View(ev);
        }

        public IActionResult Edit(long id)
        {
            var ev = repository.Events.FirstOrDefault(e => e.EventId == id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Event ev)
        {
            if (ModelState.IsValid)
            {
                var existing = repository.Events.FirstOrDefault(e => e.EventId == ev.EventId);
                if (existing == null) return NotFound();
                existing.Title = ev.Title;
                existing.Description = ev.Description;
                existing.Date = ev.Date;
                existing.Location = ev.Location;
                existing.Price = ev.Price;
                repository.SaveEvent(existing);
                TempData["message"] = $"Подію \"{ev.Title}\" успішно оновлено.";
                return RedirectToAction(nameof(Index));
            }
            return View(ev);
        }

        public IActionResult Delete(long id)
        {
            var ev = repository.Events.FirstOrDefault(e => e.EventId == id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(long id)
        {
            var ev = repository.Events.FirstOrDefault(e => e.EventId == id);
            if (ev == null) return NotFound();
            repository.DeleteEvent(ev);
            TempData["message"] = $"Подію \"{ev.Title}\" видалено.";
            return RedirectToAction(nameof(Index));
        }
    }
}
