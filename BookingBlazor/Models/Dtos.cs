using System.ComponentModel.DataAnnotations;

namespace BookingBlazor.Models
{
    // Auth
    public class RegisterDto
    {
        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пароль")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль має бути від 8 символів")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Підтвердіть пароль")]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Role { get; set; } = "User";
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пароль")]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
        public DateTime Expiration { get; set; }
    }

    // Events
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
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Назва має бути від 2 до 200 символів")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть опис події")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть дату події")]
        public DateTime Date { get; set; } = DateTime.Now.AddDays(1);

        [Required(ErrorMessage = "Вкажіть місце проведення")]
        [StringLength(200, MinimumLength = 2)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна повинна бути більше нуля")]
        public decimal Price { get; set; }
    }

    // Tickets
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

        [Required(ErrorMessage = "Введіть email покупця")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string BuyerEmail { get; set; } = string.Empty;

        [Required]
        [Range(1, 20, ErrorMessage = "Кількість місць: від 1 до 20")]
        public int SeatCount { get; set; } = 1;

        [Required]
        public long EventId { get; set; }
    }
}
