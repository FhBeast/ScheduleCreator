using Schedule.ViewModels.Employee;
using Schedule.ViewModels.Shift;
using System.ComponentModel.DataAnnotations;
using Schedule.Entities;


namespace Schedule.ViewModels.Timetable;
public class TimetableEmployeeViewModel
{
    [Required(ErrorMessage = "Поле {0} обязательно для ввода.")]
    [StringLength(50)]
    [Display(Name = "Название расписания")]
    public string? TimetableName { get; set; }

    [Display(Name = "Владелец")]
    public string? Owner { get; set; }

    public EmployeeViewModel? Employee { get; set; }

    public List<ShiftViewModel>? ShiftViewModels { get; set; }
}
