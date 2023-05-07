using System;
using System.Linq;
using System.Threading.Tasks;
using Schedule.Models;
using Schedule.Entities;
using Schedule.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Schedule.Controllers
{
    public class TimetableController : Controller
    {
        private readonly ApplicationContext _applicationContext;

        public TimetableController(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        [HttpGet]
        public IActionResult AddTimetable()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTimetable(CreateTimetableViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_applicationContext.Timetables.Any(x => x.TimetableName == model.TimetableName))
                {
                    ModelState.AddModelError("", "Расписание с таким названием уже существует");
                    return View();
                }

                var timetable = new Timetable()
                {
                    TimetableName = model.TimetableName,
                    Owner = User.Identity.Name,
                };

                _applicationContext.Add(timetable);
                await _applicationContext.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
