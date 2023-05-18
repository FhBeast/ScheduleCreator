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
    public class ShiftController : Controller
    {
        private readonly ApplicationContext _applicationContext;

        public ShiftController(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddShift(int id, TimetableViewModel model)
        {
            ViewBag.id = id;
            var timetable = _applicationContext.Timetables.Include(x => x.Shifts).FirstOrDefault(x => x.Id == id);

            if (timetable != null)
            {
                if (timetable.Shifts == null)
                {
                    timetable.Shifts = new List<Shift>();
                }

                if (model.NewShiftViewModel != null && timetable.Shifts.Count <= 3 && ModelState.IsValid)
                {
                    SaveShifts(id, model);

                    timetable.Shifts.Add(new Shift()
                    {
                        Start = model.NewShiftViewModel.Start,
                        End = model.NewShiftViewModel.End,
                        EmployeesNumber = model.NewShiftViewModel.EmployeesNumber,
                        IsQuaranteeWeekend = model.NewShiftViewModel.IsQuaranteeWeekend,
                    });

                    await _applicationContext.SaveChangesAsync();
                }

                return RedirectToAction("EditTimetable", "Timetable", new { id });
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveShift(int id, int index)
        {
            ViewBag.id = id;
            var timetable = _applicationContext.Timetables.Include(x => x.Shifts).FirstOrDefault(x => x.Id == id);

            if (timetable != null)
            {
                if (timetable.Shifts != null && timetable.Shifts.Count > index && index >= 0)
                {
                    _applicationContext.Shifts.Remove(timetable.Shifts[index]);
                    //timetable.Shifts.RemoveAt(index);
                }

                await _applicationContext.SaveChangesAsync();

                return RedirectToAction("EditTimetable", "Timetable", new { id });
            }

            return NotFound();
        }

        private void SaveShifts(int id, TimetableViewModel model)
        {
            var timetable = _applicationContext.Timetables.Include(x => x.Shifts).FirstOrDefault(x => x.Id == id);

            if (timetable != null)
            {
                var shifts = new List<Shift>();

                if (model.ShiftViewModels != null)
                {
                    foreach (var shiftViewModel in model.ShiftViewModels)
                    {
                        shifts.Add(new Shift
                        {
                            Start = shiftViewModel.Start,
                            End = shiftViewModel.End,
                            EmployeesNumber = shiftViewModel.EmployeesNumber,
                            IsQuaranteeWeekend = shiftViewModel.IsQuaranteeWeekend,
                        });
                    }
                }

                if (timetable.Shifts != null)
                {
                    _applicationContext.Shifts.RemoveRange(timetable.Shifts);
                }

                timetable.Shifts = shifts;
            }
        }
    }
}
