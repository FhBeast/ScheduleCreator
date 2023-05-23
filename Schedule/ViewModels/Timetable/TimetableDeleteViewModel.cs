using System.ComponentModel.DataAnnotations;

namespace Schedule.ViewModels.Timetable;
public class TimetableDeleteViewModel
{
    [Display(Name = "Название расписания")]
    public string? TimetableName { get; set; }

    [Display(Name = "Владелец")]
    public string? Owner { get; set; }

    [Required(ErrorMessage = "Поле {0} обязательно для ввода.")]
    [StringLength(50)]
    [Compare("TimetableName", ErrorMessage = "Названия не совпадают")]
    [Display(Name = "Потверждение названия расписания")]
    public string? TimetableNameConfirm { get; set; }
}
