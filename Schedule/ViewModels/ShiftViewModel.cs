using System.ComponentModel.DataAnnotations;

namespace Schedule.ViewModels;
public class ShiftViewModel
{
    [Required]
    [Display(Name = "Начало смены")]
    public string? Start { get; set; }
    [Required]
    [Display(Name = "Конец смены")]
    public string? End { get; set; }
    [Required]
    [Display(Name = "Количество работников")]
    public int EmployeesNumber { get; set; }
    public bool? IsQuaranteeWeekend { get; set; }
}
