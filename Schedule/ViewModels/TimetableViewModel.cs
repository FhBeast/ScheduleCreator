using System.ComponentModel.DataAnnotations;

namespace Schedule.ViewModels;
public class TimetableViewModel
{
    [Required(ErrorMessage = "Поле {0} обязательно для ввода.")]
    [StringLength(50)]
    [Display(Name = "Название расписания")]
    public string? TimetableName { get; set; }

    [Display(Name = "Владелец")]
    public string? Owner { get; set; }

    [Display(Name = "Понедельник")]
    public bool Monday { get; set; }
    [Display(Name = "Вторник")]
    public bool Tuesday { get; set; }
    [Display(Name = "Среда")]
    public bool Wednesday { get; set; }
    [Display(Name = "Четверг")]
    public bool Thursday { get; set; }
    [Display(Name = "Пятница")]
    public bool Friday { get; set; }
    [Display(Name = "Суббота")]
    public bool Saturday { get; set; }
    [Display(Name = "Воскресенье")]
    public bool Sunday { get; set; }

    public List<ShiftViewModel>? ShiftViewModels { get; set; }

    public List<EmployeeViewModel>? EmployeeViewModels { get; set; }

    public ShiftViewModel? NewShiftViewModel { get; set; }
}
