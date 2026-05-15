using System.ComponentModel.DataAnnotations;

namespace BookingApi.Models
{
    // Auth DTOs
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "User";
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
        public DateTime Expiration { get; set; }
    }

    // Event DTOs
    public class EventDto
    {
        public long? EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class CreateEventDto
    {
        [Required(ErrorMessage = "Введіть назву події")]
        [StringLength(200, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }

    // Ticket DTOs
    public class TicketDto
    {
        public long? TicketId { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public int SeatCount { get; set; }
        public DateTime OrderDate { get; set; }
        public long EventId { get; set; }
        public string? EventTitle { get; set; }
    }

    public class CreateTicketDto
    {
        [Required(ErrorMessage = "Введіть ім'я покупця")]
        [StringLength(100, MinimumLength = 2)]
        public string BuyerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string BuyerEmail { get; set; } = string.Empty;

        [Required]
        [Range(1, 20)]
        public int SeatCount { get; set; } = 1;

        [Required]
        public long EventId { get; set; }
    }
}
