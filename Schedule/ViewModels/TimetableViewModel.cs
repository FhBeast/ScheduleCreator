using System.ComponentModel.DataAnnotations;

namespace Schedule.ViewModels;
public class TimetableViewModel
{
    [Required]
    [Display(Name = "Название расписания")]
    public string? TimetableName { get; set; }

    [Display(Name = "Владелец")]
    public string? Owner { get; set; }

    [Display(Name = "Выходные дни")]
    public string? Weekend { get; set; }

    public List<ShiftViewModel> ShiftViewModels { get; set; }

    public List<EmployeeVIewModel> EmployeeVIewModels { get; set; }
}
