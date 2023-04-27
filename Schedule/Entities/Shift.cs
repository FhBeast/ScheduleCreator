namespace Schedule.Entities;
public class Shift : BaseEntity
{
    public string Start { get; set; }
    public string End { get; set; }
    public bool IsQuaranteeWeekend;
}
