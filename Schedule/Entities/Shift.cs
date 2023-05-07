namespace Schedule.Entities;
public class Shift : BaseEntity
{
    public string? Start { get; set; }
    public string? End { get; set; }
    public int EmployeesNumber { get; set; }
    public bool IsQuaranteeWeekend { get; set; }
}
