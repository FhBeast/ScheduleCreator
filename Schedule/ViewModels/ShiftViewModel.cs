using System.ComponentModel.DataAnnotations;

namespace Schedule.ViewModels;
public class ShiftViewModel
{
    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "Начало смены")]
    public string? Start { get; set; }
    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "Конец смены")]
    public string? End { get; set; }
    [Range(1, 1000000, ErrorMessage = "Недопустимое количество")]
    [Display(Name = "Количество работников")]
    public int EmployeesNumber { get; set; }
    [Display(Name = "Гарантия выходного")]
    public bool IsQuaranteeWeekend { get; set; }
}
