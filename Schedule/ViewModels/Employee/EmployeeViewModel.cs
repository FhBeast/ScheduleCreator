using System.ComponentModel.DataAnnotations;

namespace Schedule.ViewModels.Employee;
public class EmployeeViewModel
{
    [Required]
    [Display(Name = "Имя")]
    public string? FirstName { get; set; }
    [Required]
    [Display(Name = "Фамилия")]
    public string? LastName { get; set; }
    [Display(Name = "Отчество")]
    public string? Surname { get; set; }
    [EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }
    public string? WorkDays { get; set; }
}
