using Schedule.ViewModels.Employee;
using Schedule.ViewModels.Shift;
using System.ComponentModel.DataAnnotations;
using Schedule.Entities;
using Schedule.ViewModels.Timetable;

namespace Schedule.ViewModels.Search;
public class SearchViewModel
{
    [Required(ErrorMessage = "Поле {0} обязательно для ввода.")]
    [StringLength(50)]
    [Display(Name = "Поиск")]
    public string? SearchText { get; set; }
    public List<TimetableSearchingViewModel>? TimetableSearchingViewModels { get; set; }
}