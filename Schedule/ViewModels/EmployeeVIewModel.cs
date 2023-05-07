using System.ComponentModel.DataAnnotations;

namespace Schedule.ViewModels;
public class EmployeeVIewModel
{
    [Required]
    [Display(Name = "Имя")]
    public string? FirstName { get; set; }
    [Required]
    [Display(Name = "Фамилия")]
    public string? LastName { get; set; }
    [Display(Name = "Отчество")]
    public string? Surname { get; set; }
}
