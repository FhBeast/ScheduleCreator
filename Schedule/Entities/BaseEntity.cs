using System.ComponentModel.DataAnnotations;

namespace Schedule.Entities;
public class BaseEntity
{
    [Key] public int Id { get; set; }
}
