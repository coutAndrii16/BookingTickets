using BookingData.Models;
using BookingMvc.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingMvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IEventRepository repository;
        public int PageSize = 4;

        public HomeController(IEventRepository repo)
        {
            repository = repo;
        }

        public IActionResult Index(string? category, int page = 1)
        {
            var filteredEvents = repository.Events
                .Where(e => category == null || e.Location == category);

            return View(new EventListViewModel
            {
                Events = filteredEvents
                    .OrderBy(e => e.Date)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = filteredEvents.Count()
                },
                CurrentCategory = category
            });
        }
    }
}
