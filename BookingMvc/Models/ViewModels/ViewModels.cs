using System.ComponentModel.DataAnnotations;

namespace BookingMvc.Models.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        public string ReturnUrl { get; set; } = "/";
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пароль")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль має бути не коротший за 8 символів")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Підтвердіть пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження паролю")]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Оберіть роль")]
        [Display(Name = "Тип облікового запису")]
        public string Role { get; set; } = "User";
    }

    public class UserProfileModel
    {
        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Ім'я користувача")]
        public string? UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Поточний пароль")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль має бути не коротший за 8 символів")]
        [Display(Name = "Новий пароль")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження нового паролю")]
        [Compare("NewPassword", ErrorMessage = "Паролі не співпадають")]
        public string? ConfirmNewPassword { get; set; }
    }

    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
    }

    public class EventListViewModel
    {
        public IQueryable<BookingData.Models.Event> Events { get; set; } =
            Enumerable.Empty<BookingData.Models.Event>().AsQueryable();
        public PagingInfo PagingInfo { get; set; } = new PagingInfo();
        public string? CurrentCategory { get; set; }
    }
}
