using BookingApi.Models;
using BookingData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository repository;

        public EventsController(IEventRepository repo)
        {
            repository = repo;
        }

        /// <summary>
        /// GET /api/events — всі події (публічний доступ)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] string? location = null)
        {
            var query = repository.Events.AsQueryable();
            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(e => e.Location == location);

            var result = query.OrderBy(e => e.Date)
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Description = e.Description,
                    Date = e.Date,
                    Location = e.Location,
                    Price = e.Price
                }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// GET /api/events/{id} — одна подія
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(long id)
        {
            var ev = repository.Events.FirstOrDefault(e => e.EventId == id);
            if (ev == null) return NotFound(new { message = $"Подію з id={id} не знайдено" });

            return Ok(new EventDto
            {
                EventId = ev.EventId,
                Title = ev.Title,
                Description = ev.Description,
                Date = ev.Date,
                Location = ev.Location,
                Price = ev.Price
            });
        }

        /// <summary>
        /// POST /api/events — створити подію (тільки Admin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateEventDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ev = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                Date = dto.Date,
                Location = dto.Location,
                Price = dto.Price
            };

            repository.CreateEvent(ev);

            return CreatedAtAction(nameof(GetById), new { id = ev.EventId },
                new EventDto
                {
                    EventId = ev.EventId,
                    Title = ev.Title,
                    Description = ev.Description,
                    Date = ev.Date,
                    Location = ev.Location,
                    Price = ev.Price
                });
        }

        /// <summary>
        /// PUT /api/events/{id} — оновити подію (тільки Admin)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(long id, [FromBody] CreateEventDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ev = repository.Events.FirstOrDefault(e => e.EventId == id);
            if (ev == null) return NotFound(new { message = $"Подію з id={id} не знайдено" });

            ev.Title = dto.Title;
            ev.Description = dto.Description;
            ev.Date = dto.Date;
            ev.Location = dto.Location;
            ev.Price = dto.Price;

            repository.SaveEvent(ev);

            return Ok(new EventDto
            {
                EventId = ev.EventId,
                Title = ev.Title,
                Description = ev.Description,
                Date = ev.Date,
                Location = ev.Location,
                Price = ev.Price
            });
        }

        /// <summary>
        /// DELETE /api/events/{id} — видалити подію (тільки Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(long id)
        {
            var ev = repository.Events.FirstOrDefault(e => e.EventId == id);
            if (ev == null) return NotFound(new { message = $"Подію з id={id} не знайдено" });

            repository.DeleteEvent(ev);
            return NoContent();
        }
    }
}
