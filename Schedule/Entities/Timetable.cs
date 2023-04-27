using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace Schedule.Entities;

[Index(nameof(TimetableName), IsUnique = true)]
public class Timetable : BaseEntity
{
    public string TimetableName { get; set; }
    public string? Weekend { get; set; }
    public List<Shift> Shifts { get; set; }
    public List<Employee> Employees { get; set; }
}
