using BookingData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookingMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTicketController : Controller
    {
        private readonly ITicketRepository ticketRepository;
        private readonly IEventRepository eventRepository;

        public AdminTicketController(ITicketRepository ticketRepo, IEventRepository eventRepo)
        {
            ticketRepository = ticketRepo;
            eventRepository = eventRepo;
        }

        public IActionResult Index()
        {
            var tickets = ticketRepository.Tickets
                .Include(t => t.Event)
                .OrderByDescending(t => t.OrderDate);
            return View(tickets);
        }

        public IActionResult Details(long id)
        {
            var ticket = ticketRepository.Tickets
                .Include(t => t.Event)
                .FirstOrDefault(t => t.TicketId == id);
            if (ticket == null) return NotFound();
            return View(ticket);
        }

        public IActionResult Create(long? eventId)
        {
            PopulateEventsDropDown(eventId);
            return View(new Ticket { OrderDate = DateTime.Now, EventId = eventId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.OrderDate = DateTime.Now;
                ticketRepository.CreateTicket(ticket);
                TempData["message"] = $"Квиток для \"{ticket.BuyerName}\" успішно створено.";
                return RedirectToAction(nameof(Index));
            }
            PopulateEventsDropDown(ticket.EventId);
            return View(ticket);
        }

        public IActionResult Edit(long id)
        {
            var ticket = ticketRepository.Tickets.FirstOrDefault(t => t.TicketId == id);
            if (ticket == null) return NotFound();
            PopulateEventsDropDown(ticket.EventId);
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var existing = ticketRepository.Tickets.FirstOrDefault(t => t.TicketId == ticket.TicketId);
                if (existing == null) return NotFound();
                existing.BuyerName = ticket.BuyerName;
                existing.BuyerEmail = ticket.BuyerEmail;
                existing.SeatCount = ticket.SeatCount;
                existing.EventId = ticket.EventId;
                ticketRepository.SaveTicket(existing);
                TempData["message"] = $"Квиток для \"{ticket.BuyerName}\" оновлено.";
                return RedirectToAction(nameof(Index));
            }
            PopulateEventsDropDown(ticket.EventId);
            return View(ticket);
        }

        public IActionResult Delete(long id)
        {
            var ticket = ticketRepository.Tickets
                .Include(t => t.Event)
                .FirstOrDefault(t => t.TicketId == id);
            if (ticket == null) return NotFound();
            return View(ticket);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(long id)
        {
            var ticket = ticketRepository.Tickets.FirstOrDefault(t => t.TicketId == id);
            if (ticket == null) return NotFound();
            ticketRepository.DeleteTicket(ticket);
            TempData["message"] = "Квиток успішно видалено.";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateEventsDropDown(long? selectedId = null)
        {
            ViewBag.EventsList = eventRepository.Events
                .OrderBy(e => e.Title)
                .Select(e => new SelectListItem
                {
                    Value = e.EventId.ToString(),
                    Text = $"{e.Title} ({e.Date:dd.MM.yyyy}) — {e.Location}",
                    Selected = e.EventId == selectedId
                }).ToList();
        }
    }
}
