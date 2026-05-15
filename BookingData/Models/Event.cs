using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingData.Models
{
    public class Event
    {
        public long? EventId { get; set; }

        [Required(ErrorMessage = "Введіть назву події")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Назва має бути від 2 до 200 символів")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть опис події")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть дату події")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Вкажіть місце проведення")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Місце має бути від 2 до 200 символів")]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна повинна бути більше нуля")]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Price { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
