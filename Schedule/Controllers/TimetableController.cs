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

        [Authorize]
        [HttpGet]
        public IActionResult AddTimetable()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddTimetable(TimetableCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_applicationContext.Timetables.Any(x => x.TimetableName == model.TimetableName))
                {
                    ModelState.AddModelError("", "Расписание с таким названием уже существует");
                    return View();
                }

                var username = User?.Identity?.Name;
                var timetable = new Timetable()
                {
                    TimetableName = model.TimetableName,
                    Owner = username,
                    Weekend = "0000000",
                };

                _applicationContext.Add(timetable);
                await _applicationContext.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult EditTimetable(int id)
        {
            ViewBag.id = id;
            var timetable = _applicationContext.Timetables.Include(x => x.Shifts).FirstOrDefault(x => x.Id == id);
            var username = User?.Identity?.Name;

            if (timetable != null && timetable.Owner == username)
            {
                ViewBag.timetableName = timetable.TimetableName;
                ViewBag.owner = timetable.Owner;

                var shiftViewModels = new List<ShiftViewModel>();

                if (timetable.Shifts != null)
                {
                    foreach (var shift in timetable.Shifts)
                    {
                        shiftViewModels.Add(new ShiftViewModel
                        {
                            Start = shift.Start,
                            End = shift.End,
                            EmployeesNumber = shift.EmployeesNumber,
                            IsQuaranteeWeekend = shift.IsQuaranteeWeekend,
                        });
                    }
                }

                var timetableViewModel = new TimetableViewModel()
                {
                    TimetableName = timetable.TimetableName,
                    Owner = timetable.Owner,
                    Monday = timetable?.Weekend?[0] == '1',
                    Tuesday = timetable?.Weekend?[1] == '1',
                    Wednesday = timetable?.Weekend?[2] == '1',
                    Thursday = timetable?.Weekend?[3] == '1',
                    Friday = timetable?.Weekend?[4] == '1',
                    Saturday = timetable?.Weekend?[5] == '1',
                    Sunday = timetable?.Weekend?[6] == '1',
                    ShiftViewModels = shiftViewModels,
                };

                if (shiftViewModels.Count <= 3)
                {
                    timetableViewModel.NewShiftViewModel = new ShiftViewModel();
                }

                return View(timetableViewModel);
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditTimetable(int id, TimetableViewModel model)
        {
            ViewBag.id = id;

            var timetable = _applicationContext.Timetables.Include(x => x.Shifts).FirstOrDefault(x => x.Id == id);
            var username = User?.Identity?.Name;

            if (timetable != null && timetable.Owner == username)
            {
                ViewBag.timetableName = timetable.TimetableName;
                ViewBag.owner = timetable.Owner;

                ModelState.Remove("NewShiftViewModel.Start");
                ModelState.Remove("NewShiftViewModel.End");
                ModelState.Remove("NewShiftViewModel.EmployeesNumber");
                
                if (ModelState.IsValid)
                {
                    if (timetable.TimetableName != model.TimetableName
                        && _applicationContext.Timetables.Any(x => x.TimetableName == model.TimetableName))
                    {
                        ModelState.AddModelError("", "Расписание с таким названием уже существует");
                        return View(model);
                    }

                    bool[] weekendArray = new bool[]
                    {
                        model.Monday,
                        model.Tuesday,
                        model.Wednesday,
                        model.Thursday,
                        model.Friday,
                        model.Saturday,
                        model.Sunday,
                    };

                    string weekendString = string.Join("", weekendArray.Select(b => b ? "1" : "0"));

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

                    timetable.TimetableName = model.TimetableName;
                    timetable.Weekend = weekendString;
                    timetable.Shifts = shifts;

                    //_applicationContext.Update(timetable);

                    await _applicationContext.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }

                return View(model);
            }

            return NotFound();

            //if (ModelState.IsValid)
            //{
            //    var timetable = _applicationContext.Timetables.Find(id);
            //    var username = User?.Identity?.Name;

            //    if (timetable != null && timetable.Owner == username)
            //    {
            //        ViewBag.timetableName = timetable.TimetableName;
            //        ViewBag.owner = timetable.Owner;

            //        if (timetable.TimetableName != model.TimetableName
            //            && _applicationContext.Timetables.Any(x => x.TimetableName == model.TimetableName))
            //        {
            //            ModelState.AddModelError("", "Расписание с таким названием уже существует");
            //            return View(model);
            //        }

            //        bool[] weekendArray = new bool[]
            //        {
            //            model.Monday,
            //            model.Tuesday,
            //            model.Wednesday,
            //            model.Thursday,
            //            model.Friday,
            //            model.Saturday,
            //            model.Sunday,
            //        };

            //        string weekendString = string.Join("", weekendArray.Select(b => b ? "1" : "0"));

            //        timetable.TimetableName = model.TimetableName;
            //        timetable.Weekend = weekendString;

            //        _applicationContext.Update(timetable);

            //        await _applicationContext.SaveChangesAsync();
            //        return RedirectToAction("Index", "Home");
            //    }

            //    return NotFound();
            //}

            //return View(model);
        }        

        [Authorize]
        [HttpGet]
        public IActionResult DeleteTimetable(int id)
        {
            ViewBag.id = id;
            var timetable = _applicationContext.Timetables.Find(id);
            var username = User?.Identity?.Name;

            if (timetable != null && timetable.Owner == username)
            {
                var timetableDeleteViewModel = new TimetableDeleteViewModel()
                {
                    TimetableName = timetable.TimetableName,
                    Owner = timetable.Owner,
                };

                return View(timetableDeleteViewModel);
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteTimetable(int id, TimetableDeleteViewModel model)
        {
            ViewBag.id = id;
            var timetable = _applicationContext.Timetables.Include(x => x.Shifts).Include(x => x.Employees).FirstOrDefault(x => x.Id == id);
            var username = User?.Identity?.Name;

            if (timetable != null && timetable.Owner == username)
            {
                if (ModelState.IsValid)
                {
                    if (timetable.Shifts != null)
                    {
                        _applicationContext.Shifts.RemoveRange(timetable.Shifts);
                    }

                    _applicationContext.Remove(timetable);
                    await _applicationContext.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }

                return View(model);
            }

            return NotFound();
        }
    }
}
