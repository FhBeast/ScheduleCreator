using Microsoft.AspNetCore.Mvc;
using Schedule.Models;
using Schedule.Entities;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Schedule.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationContext _applicationContext;

    public HomeController(ILogger<HomeController> logger, ApplicationContext applicationContext)
    {
        _logger = logger;
        _applicationContext = applicationContext;
    }

    public IActionResult Index()
    {
        var username = User?.Identity?.Name;
        var timetables = _applicationContext.Timetables
            .Include(x => x.Shifts)
            .Include(x => x.Employees)
            .ToList()
            .Where(x => x.Owner == username);

        return View(timetables);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}