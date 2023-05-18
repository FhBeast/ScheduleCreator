using System.ComponentModel.DataAnnotations;

namespace Schedule.ViewModels;
public class RegisterViewModel
{        
    [Required(ErrorMessage = "Поле {0} обязательно для ввода.")]
    [StringLength(60)]
    [EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Поле {0} обязательно для ввода.")]
    [StringLength(50)]
    [Display(Name = "Имя")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Поле {0} обязательно для ввода.")]
    [StringLength(50)]
    [Display(Name = "Фамилия")]
    public string? LastName { get; set; }

    [StringLength(50)]
    [Display(Name = "Отчество")]
    public string? Surname { get; set; }

    [Required(ErrorMessage = "Поле {0} обязательно для ввода.")]
    [StringLength(30, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Поле {0} обязательно для ввода.")]
    [StringLength(30)]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтвердить пароль")]
    public string? PasswordConfirm { get; set; }
}
