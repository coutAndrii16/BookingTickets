using BookingData.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookingMvc.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private readonly IEventRepository repository;

        public NavigationMenuViewComponent(IEventRepository repo)
        {
            repository = repo;
        }

        public IViewComponentResult Invoke()
        {
            var categories = repository.Events
                .Select(e => e.Location)
                .Distinct()
                .OrderBy(x => x);
            return View(categories);
        }
    }
}
