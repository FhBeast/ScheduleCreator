using System.ComponentModel.DataAnnotations;

namespace Schedule.ViewModels;
public class CreateTimetableViewModel
{
    [Required]
    [Display(Name = "Название расписания")]
    public string? TimetableName { get; set; }

    [Display(Name = "Владелец")]
    public string? Owner { get; set; }
}

