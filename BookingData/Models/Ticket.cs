using System.ComponentModel.DataAnnotations;

namespace BookingData.Models
{
    public class Ticket
    {
        public long? TicketId { get; set; }

        [Required(ErrorMessage = "Введіть ім'я покупця")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ім'я має бути від 2 до 100 символів")]
        [Display(Name = "Ім'я покупця")]
        public string BuyerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть email покупця")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [Display(Name = "Email покупця")]
        public string BuyerEmail { get; set; } = string.Empty;

        [Required]
        [Range(1, 20, ErrorMessage = "Кількість місць: від 1 до 20")]
        [Display(Name = "Кількість місць")]
        public int SeatCount { get; set; } = 1;

        [Display(Name = "Дата замовлення")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Вкажіть подію")]
        [Display(Name = "Подія")]
        public long EventId { get; set; }

        public Event? Event { get; set; }
    }
}
