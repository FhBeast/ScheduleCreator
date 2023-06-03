using Schedule.ViewModels.Employee;
using Schedule.ViewModels.Shift;
using System.ComponentModel.DataAnnotations;
using Schedule.Entities;

namespace Schedule.ViewModels.Timetable;
public class TimetableSearchingViewModel
{
    [Display(Name = "Название расписания")]
    public string? TimetableName { get; set; }

    [Display(Name = "Владелец")]
    public string? Owner { get; set; }

    [Display(Name = "Количество смен")]
    public int ShiftsNumber { get; set; }

    [Display(Name = "Количество работников")]
    public int EmployeesNumber { get; set; }
}
