using System.ComponentModel.DataAnnotations.Schema;

namespace Schedule.Entities;
public class Employee : BaseEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public int ExceedingWorkingDays { get; set; }
    public string? WorkDays { get; set; }
}
