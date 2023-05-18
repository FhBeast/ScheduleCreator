using System.ComponentModel.DataAnnotations;

namespace Schedule.ViewModels;
public class TimetableCreateViewModel
{
    [Required(ErrorMessage = "Поле {0} обязательно для ввода.")]
    [StringLength(50)]
    [Display(Name = "Название расписания")]
    public string? TimetableName { get; set; }

    [Display(Name = "Владелец")]
    public string? Owner { get; set; }
}

