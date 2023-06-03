using System;
using System.Linq;
using System.Threading.Tasks;
using Schedule.Models;
using Schedule.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Schedule.ViewModels.Search;
using Schedule.ViewModels.Timetable;
using Schedule.ViewModels.Employee;
using Schedule.ViewModels.Shift;
using Schedule.Services;

namespace Schedule.Controllers;
public class SearchController : Controller
{
    private readonly ApplicationContext _applicationContext;

    public SearchController(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    [Authorize]
    [HttpGet]
    public IActionResult Index()
    {
        return View(new SearchViewModel());
    }

    [Authorize]
    [HttpPost]
    public IActionResult Index(SearchViewModel model)
    {
        if (model.SearchText != null)
        {
            var username = User.Identity?.Name;
            var timetables = _applicationContext.Timetables
                .Where(x => x.TimetableName != null && x.Owner != username && x.TimetableName
                .ToLower()
                .Contains(model.SearchText
                .ToLower()));

            model.TimetableSearchingViewModels = timetables.Select(x => new TimetableSearchingViewModel()
            {
                TimetableName = x.TimetableName,
                Owner = x.Owner,
                EmployeesNumber = x.Employees.Count(),
                ShiftsNumber = x.Shifts.Count(),
            }).ToList();
        }

        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> TrackTimetable(string name, SearchViewModel model)
    {
        var username = User.Identity?.Name;

        var timetable = _applicationContext.Timetables
            .Include(x => x.Tracks)
            .FirstOrDefault(x => x.TimetableName == name);
        var user = _applicationContext.Users
            .Include(x => x.Tracks)
            .FirstOrDefault(x => x.UserName == username);
        

        var track = new Tracking();

        if (timetable != null && timetable.Tracks != null && user != null)
        {
            if (timetable.Owner == username || timetable.Tracks.Any(x => user.Tracks.Contains(x)))
            {
                return RedirectToAction("Index", "Home");
            }

            timetable.Tracks.Add(track);
            user.Tracks.Add(track);

            await _applicationContext.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Home");
    }
}
