using Schedule.Entities;
using Microsoft.AspNetCore.Identity;

namespace Schedule.Models;
public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Surname { get; set; }
}
