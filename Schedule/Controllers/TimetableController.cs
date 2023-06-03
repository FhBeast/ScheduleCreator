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
using Schedule.ViewModels.Timetable;
using Schedule.ViewModels.Employee;
using Schedule.ViewModels.Shift;
using Schedule.Services;

namespace Schedule.Controllers;
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
        var timetable = _applicationContext.Timetables
            .Include(x => x.Shifts)
            .Include(x => x.Employees)
            .FirstOrDefault(x => x.Id == id);
        var username = User?.Identity?.Name;

        if (timetable != null && timetable.Owner == username)
        {
            ViewBag.timetableName = timetable.TimetableName;
            ViewBag.owner = timetable.Owner;

            var shiftViewModels = new List<ShiftViewModel>();
            var employeeViewModels = new List<EmployeeViewModel>();

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

            if (timetable.Employees != null)
            {
                foreach (var employee in timetable.Employees)
                {
                    employeeViewModels.Add(new EmployeeViewModel
                    {
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        Surname = employee.Surname,
                        Email = employee.Email,
                        WorkDays = employee.WorkDays,
                    }); ;
                }
            }

            var model = new TimetableViewModel()
            {
                TimetableName = timetable.TimetableName,
                Owner = timetable.Owner,
                Monday = timetable.Weekend?[0] == '1',
                Tuesday = timetable.Weekend?[1] == '1',
                Wednesday = timetable.Weekend?[2] == '1',
                Thursday = timetable.Weekend?[3] == '1',
                Friday = timetable.Weekend?[4] == '1',
                Saturday = timetable.Weekend?[5] == '1',
                Sunday = timetable.Weekend?[6] == '1',
                ShiftViewModels = shiftViewModels,
                EmployeeViewModels = employeeViewModels,
                NewEmployeeViewModel = new EmployeeViewModel(),
                State = timetable.State,
            };

            if (shiftViewModels.Count <= 3)
            {
                model.NewShiftViewModel = new ShiftViewModel();
            }

            return View(model);
        }

        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditTimetable(int id, TimetableViewModel model)
    {
        ViewBag.id = id;

        var result = await SaveChanges(id, model);

        if (result == SaveResult.Success)
        {
            model.State = TimetableState.UpdateRequired;
            return View(model);
        }
        else if (result == SaveResult.Failure)
        {
            return View(model);
        }
        else
        {
            return NotFound();
        }
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
        var timetable = _applicationContext.Timetables
            .Include(x => x.Shifts)
            .Include(x => x.Employees)
            .FirstOrDefault(x => x.Id == id);
        var username = User?.Identity?.Name;

        if (timetable != null && timetable.Owner == username)
        {
            if (ModelState.IsValid)
            {
                if (timetable.Shifts != null)
                {
                    _applicationContext.Shifts.RemoveRange(timetable.Shifts);
                }

                if (timetable.Employees != null)
                {
                    _applicationContext.Employees.RemoveRange(timetable.Employees);
                }

                if (timetable.Tracks != null)
                {
                    _applicationContext.Tracks.RemoveRange(timetable.Tracks);
                }

                _applicationContext.Remove(timetable);
                await _applicationContext.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddShiftToTimetable(int id, TimetableViewModel model)
    {
        ViewBag.id = id;

        if (model.ShiftViewModels == null)
        {
            model.ShiftViewModels = new List<ShiftViewModel>();
        }

        if (model.NewShiftViewModel != null && model.ShiftViewModels.Count <= 3 &&
            ModelState["NewShiftViewModel.Start"]?.ValidationState == ModelValidationState.Valid &&
            ModelState["NewShiftViewModel.End"]?.ValidationState == ModelValidationState.Valid &&
            ModelState["NewShiftViewModel.EmployeesNumber"]?.ValidationState == ModelValidationState.Valid)
        {
            model.ShiftViewModels.Add(new ShiftViewModel()
            {
                Start = model.NewShiftViewModel.Start,
                End = model.NewShiftViewModel.End,
                EmployeesNumber = model.NewShiftViewModel.EmployeesNumber,
                IsQuaranteeWeekend = model.NewShiftViewModel.IsQuaranteeWeekend,
            });
        }

        if (await SaveChanges(id, model) == SaveResult.NotFound)
        {
            return NotFound();
        }

        return RedirectToAction("EditTimetable", new { id });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RemoveShiftToTimetable(int id, TimetableViewModel model, int index)
    {
        ViewBag.id = id;

        if (model.ShiftViewModels != null)
        {
            model.ShiftViewModels.Remove(model.ShiftViewModels[index]);
        }

        if (await SaveChanges(id, model) == SaveResult.NotFound)
        {
            return NotFound();
        }

        return RedirectToAction("EditTimetable", new { id });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddEmployeeToTimetable(int id, TimetableViewModel model)
    {
        ViewBag.id = id;

        if (model.EmployeeViewModels == null)
        {
            model.EmployeeViewModels = new List<EmployeeViewModel>();
        }

        if (model.NewEmployeeViewModel != null &&
            ModelState[$"NewEmployeeViewModel.FirstName"]?.ValidationState == ModelValidationState.Valid &&
            ModelState[$"NewEmployeeViewModel.LastName"]?.ValidationState == ModelValidationState.Valid &&
            ModelState[$"NewEmployeeViewModel.Surname"]?.ValidationState == ModelValidationState.Valid &&
            ModelState[$"NewEmployeeViewModel.Email"]?.ValidationState == ModelValidationState.Valid)
        {
            model.EmployeeViewModels.Add(new EmployeeViewModel()
            {
                FirstName = model.NewEmployeeViewModel.FirstName,
                LastName = model.NewEmployeeViewModel.LastName,
                Surname = model.NewEmployeeViewModel.Surname,
                Email = model.NewEmployeeViewModel.Email,
            });
        }

        if (await SaveChanges(id, model) == SaveResult.NotFound)
        {
            return NotFound();
        }

        return RedirectToAction("EditTimetable", new { id });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RemoveEmployeeToTimetable(int id, TimetableViewModel model, int index)
    {
        ViewBag.id = id;

        if (model.EmployeeViewModels != null)
        {
            model.EmployeeViewModels.Remove(model.EmployeeViewModels[index]);
        }

        if (await SaveChanges(id, model) == SaveResult.NotFound)
        {
            return NotFound();
        }

        return RedirectToAction("EditTimetable", new { id });
    }

    private async Task<SaveResult> SaveChanges(int id, TimetableViewModel model)
    {
        var timetable = _applicationContext.Timetables
            .Include(x => x.Shifts)
            .Include(x => x.Employees)
            .FirstOrDefault(x => x.Id == id);
        var username = User?.Identity?.Name;

        SaveResult result = SaveResult.NotFound;

        if (timetable != null && timetable.Owner == username)
        {
            result = SaveResult.Success;

            if (timetable.TimetableName != model.TimetableName
                && _applicationContext.Timetables.Any(x => x.TimetableName == model.TimetableName))
            {
                ModelState.AddModelError("", "Расписание с таким названием уже существует");
                result = SaveResult.Failure;
            }
            else if (ModelState["TimetableName"]?.ValidationState == ModelValidationState.Valid)
            {
                timetable.TimetableName = model.TimetableName;
            }
            else
            {
                result = SaveResult.Failure;
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
            timetable.Weekend = weekendString;

            var shifts = new List<Shift>();

            if (model.ShiftViewModels != null)
            {
                for (int i = 0; i < model.ShiftViewModels.Count; i++)
                {
                    if (ModelState[$"ShiftViewModels[{i}].Start"]?.ValidationState == ModelValidationState.Valid &&
                        ModelState[$"ShiftViewModels[{i}].End"]?.ValidationState == ModelValidationState.Valid &&
                        ModelState[$"ShiftViewModels[{i}].EmployeesNumber"]?.ValidationState == ModelValidationState.Valid)
                    {
                        shifts.Add(new Shift
                        {
                            Start = model.ShiftViewModels[i].Start,
                            End = model.ShiftViewModels[i].End,
                            EmployeesNumber = model.ShiftViewModels[i].EmployeesNumber,
                            IsQuaranteeWeekend = model.ShiftViewModels[i].IsQuaranteeWeekend,
                        });
                    }
                    else
                    {
                        if (timetable.Shifts != null)
                        {
                            if (timetable.Shifts.Count <= i && model.NewShiftViewModel != null)
                            {
                                shifts.Add(new Shift
                                {
                                    Start = model.NewShiftViewModel.Start,
                                    End = model.NewShiftViewModel.End,
                                    EmployeesNumber = model.NewShiftViewModel.EmployeesNumber,
                                    IsQuaranteeWeekend = model.NewShiftViewModel.IsQuaranteeWeekend,
                                });
                            }
                            else
                            {
                                shifts.Add(new Shift
                                {
                                    Start = timetable.Shifts[i].Start,
                                    End = timetable.Shifts[i].End,
                                    EmployeesNumber = timetable.Shifts[i].EmployeesNumber,
                                    IsQuaranteeWeekend = timetable.Shifts[i].IsQuaranteeWeekend,
                                });

                                result = SaveResult.Failure;
                            }
                        }
                    }
                }
            }

            if (timetable.Shifts != null)
            {
                _applicationContext.Shifts.RemoveRange(timetable.Shifts);
            }

            timetable.Shifts = shifts;

            var employees = new List<Employee>();

            if (model.EmployeeViewModels != null)
            {
                for (int i = 0; i < model.EmployeeViewModels.Count; i++)
                {
                    if (ModelState[$"EmployeeViewModels[{i}].FirstName"]?.ValidationState == ModelValidationState.Valid &&
                        ModelState[$"EmployeeViewModels[{i}].LastName"]?.ValidationState == ModelValidationState.Valid &&
                        ModelState[$"EmployeeViewModels[{i}].Surname"]?.ValidationState == ModelValidationState.Valid &&
                        ModelState[$"EmployeeViewModels[{i}].Email"]?.ValidationState == ModelValidationState.Valid)
                    {
                        employees.Add(new Employee
                        {
                            FirstName = model.EmployeeViewModels[i].FirstName,
                            LastName = model.EmployeeViewModels[i].LastName,
                            Surname = model.EmployeeViewModels[i].Surname,
                            Email = model.EmployeeViewModels[i].Email,
                        });
                    }
                    else
                    {
                        if (timetable.Employees != null)
                        {
                            if (timetable.Employees.Count <= i && model.NewEmployeeViewModel != null)
                            {
                                employees.Add(new Employee
                                {
                                    FirstName = model.NewEmployeeViewModel.FirstName,
                                    LastName = model.NewEmployeeViewModel.LastName,
                                    Surname = model.NewEmployeeViewModel.Surname,
                                    Email = model.NewEmployeeViewModel.Email,
                                });
                            }
                            else
                            {
                                employees.Add(new Employee
                                {
                                    FirstName = timetable.Employees[i].FirstName,
                                    LastName = timetable.Employees[i].LastName,
                                    Surname = timetable.Employees[i].Surname,
                                    Email = timetable.Employees[i].Email,
                                });

                                result = SaveResult.Failure;
                            }
                        }
                    }
                }
            }

            if (timetable.Employees != null)
            {
                _applicationContext.Employees.RemoveRange(timetable.Employees);
            }

            timetable.Employees = employees;
            timetable.State = TimetableState.UpdateRequired;

            await _applicationContext.SaveChangesAsync();
        }

        return result;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> GenerateTimetable(int id, TimetableViewModel model)
    {
        ViewBag.id = id;

        var result = await SaveChanges(id, model);

        if (result == SaveResult.Success)
        {
            var timetable = _applicationContext.Timetables
                .Include(x => x.Shifts)
                .Include(x => x.Employees)
                .FirstOrDefault(x => x.Id == id);

            if (timetable != null)
            {
                TimetableGenerator.Generate(timetable);
                await _applicationContext.SaveChangesAsync();
            }

            return RedirectToAction("EditTimetable", new { id });
        }
        else if (result == SaveResult.Failure)
        {
            return RedirectToAction("EditTimetable", new { id });
        }
        else
        {
            return NotFound();
        }
    }

    [Authorize]
    [HttpGet]
    public IActionResult ViewTimetable(int id)
    {
        ViewBag.id = id;

        var timetable = _applicationContext.Timetables
                .Include(x => x.Shifts)
                .Include(x => x.Employees)
                .FirstOrDefault(x => x.Id == id);

        var username = User.Identity?.Name;

        if (timetable != null && timetable.Employees != null && timetable.Shifts != null)
        {

            var timetableEmpViewModel = new TimetableEmployeeViewModel()
            {
                TimetableName = timetable.TimetableName,
                Owner = timetable.Owner,
                Employee = new EmployeeViewModel(),
            };

            var employee = timetable.Employees.FirstOrDefault(x => x.Email == username);

            if (employee != null)
            {
                timetableEmpViewModel.Employee.FirstName = employee.FirstName;
                timetableEmpViewModel.Employee.LastName = employee.LastName;
                timetableEmpViewModel.Employee.Surname = employee.Surname;
                timetableEmpViewModel.Employee.WorkDays = employee.WorkDays;
            }

            timetableEmpViewModel.ShiftViewModels = timetable.Shifts.Select(x => new ShiftViewModel()
            {
                Start = x.Start,
                End = x.End,
            }).ToList();

            return View(timetableEmpViewModel);
        }

        return NotFound();
    }

    public async Task<IActionResult> UntrackTimetable(int id)
    {
        var username = User.Identity?.Name;

        var timetable = _applicationContext.Timetables
            .Include(x => x.Tracks)
            .FirstOrDefault(x => x.Id == id);
        var user = _applicationContext.Users
            .Include(x => x.Tracks)
            .FirstOrDefault(x => x.UserName == username);

        if (user != null && timetable != null)
        {
            var tracks = user.Tracks.Intersect(timetable.Tracks);

            _applicationContext.RemoveRange(tracks);
            await _applicationContext.SaveChangesAsync();
        }        

        return RedirectToAction("Index", "Home");
    }
}
