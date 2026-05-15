using BookingApi.Models;
using BookingData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketRepository ticketRepository;
        private readonly IEventRepository eventRepository;

        public TicketsController(ITicketRepository ticketRepo, IEventRepository eventRepo)
        {
            ticketRepository = ticketRepo;
            eventRepository = eventRepo;
        }

        /// <summary>
        /// GET /api/tickets — всі квитки (тільки Admin)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var tickets = ticketRepository.Tickets
                .Include(t => t.Event)
                .OrderByDescending(t => t.OrderDate)
                .Select(t => new TicketDto
                {
                    TicketId = t.TicketId,
                    BuyerName = t.BuyerName,
                    BuyerEmail = t.BuyerEmail,
                    SeatCount = t.SeatCount,
                    OrderDate = t.OrderDate,
                    EventId = t.EventId,
                    EventTitle = t.Event != null ? t.Event.Title : null
                }).ToList();

            return Ok(tickets);
        }

        /// <summary>
        /// GET /api/tickets/{id}
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(long id)
        {
            var ticket = ticketRepository.Tickets
                .Include(t => t.Event)
                .FirstOrDefault(t => t.TicketId == id);

            if (ticket == null)
                return NotFound(new { message = $"Квиток з id={id} не знайдено" });

            return Ok(new TicketDto
            {
                TicketId = ticket.TicketId,
                BuyerName = ticket.BuyerName,
                BuyerEmail = ticket.BuyerEmail,
                SeatCount = ticket.SeatCount,
                OrderDate = ticket.OrderDate,
                EventId = ticket.EventId,
                EventTitle = ticket.Event?.Title
            });
        }

        /// <summary>
        /// POST /api/tickets — купити квиток (будь-який авторизований користувач)
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] CreateTicketDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var eventExists = eventRepository.Events.Any(e => e.EventId == dto.EventId);
            if (!eventExists)
                return BadRequest(new { message = $"Подію з id={dto.EventId} не знайдено" });

            var ticket = new Ticket
            {
                BuyerName = dto.BuyerName,
                BuyerEmail = dto.BuyerEmail,
                SeatCount = dto.SeatCount,
                EventId = dto.EventId,
                OrderDate = DateTime.Now
            };

            ticketRepository.CreateTicket(ticket);

            return CreatedAtAction(nameof(GetById), new { id = ticket.TicketId },
                new TicketDto
                {
                    TicketId = ticket.TicketId,
                    BuyerName = ticket.BuyerName,
                    BuyerEmail = ticket.BuyerEmail,
                    SeatCount = ticket.SeatCount,
                    OrderDate = ticket.OrderDate,
                    EventId = ticket.EventId
                });
        }

        /// <summary>
        /// PUT /api/tickets/{id} — оновити квиток (тільки Admin)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(long id, [FromBody] CreateTicketDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ticket = ticketRepository.Tickets.FirstOrDefault(t => t.TicketId == id);
            if (ticket == null)
                return NotFound(new { message = $"Квиток з id={id} не знайдено" });

            ticket.BuyerName = dto.BuyerName;
            ticket.BuyerEmail = dto.BuyerEmail;
            ticket.SeatCount = dto.SeatCount;
            ticket.EventId = dto.EventId;

            ticketRepository.SaveTicket(ticket);

            return Ok(new TicketDto
            {
                TicketId = ticket.TicketId,
                BuyerName = ticket.BuyerName,
                BuyerEmail = ticket.BuyerEmail,
                SeatCount = ticket.SeatCount,
                OrderDate = ticket.OrderDate,
                EventId = ticket.EventId
            });
        }

        /// <summary>
        /// DELETE /api/tickets/{id} — видалити квиток (тільки Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(long id)
        {
            var ticket = ticketRepository.Tickets.FirstOrDefault(t => t.TicketId == id);
            if (ticket == null)
                return NotFound(new { message = $"Квиток з id={id} не знайдено" });

            ticketRepository.DeleteTicket(ticket);
            return NoContent();
        }
    }
}
