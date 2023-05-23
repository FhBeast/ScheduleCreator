using System.ComponentModel.DataAnnotations;

namespace Schedule.ViewModels.Authorization;
public class LoginViewModel
{
    [Required(ErrorMessage = "Поле {0} обязательно для ввода.")]
    [StringLength(60)]
    [EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Поле {0} обязательно для ввода.")]
    [StringLength(30, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string? Password { get; set; }

    [Display(Name = "Запомнить?")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
