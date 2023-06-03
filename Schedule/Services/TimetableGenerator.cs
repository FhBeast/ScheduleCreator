using Schedule.Entities;

namespace Schedule.Services;
public static class TimetableGenerator
{
    private static int daysInWeek = 7;
    private static int weeksNumber = 1;
    public static GeneratingResult Generate(Timetable timetable)
    {
        var result = GeneratingResult.Success;

        if (timetable == null ||
            timetable.Shifts == null ||
            timetable.Employees == null ||
            timetable.Weekend == null)
        {
            return GeneratingResult.Failure;
        }

        var shiftsPerDay = timetable.Shifts.Count;
        var employeesNumber = timetable.Employees.Count;
        var allowedShiftsPerDayNumber = 1;

        var shiftsNumber = shiftsPerDay * daysInWeek * weeksNumber;

        bool[] weekendMask = GetWeekendMask(timetable.Weekend);
        bool[] guaranteeMask = GetGuaranteeMask(timetable.Shifts);
        int[] requiredEmployeesNumberArr = GetRequiredEmployeesNumberArr(timetable.Shifts);

        int[,] workingField = GetWorkingField(employeesNumber, shiftsNumber, weekendMask, allowedShiftsPerDayNumber);

        result = GenerateField(workingField, weekendMask, guaranteeMask, requiredEmployeesNumberArr);
        SaveTimetable(timetable, workingField);

        if (result == GeneratingResult.Success)
        {
            timetable.State = TimetableState.Formed;
        }
        else
        {
            timetable.State = TimetableState.Error;
        }

        return result;
    }

    private static bool[] GetWeekendMask(string weekend)
    {
        return weekend.Select(x => x == '1').ToArray();
    }

    private static bool[] GetGuaranteeMask(List<Shift> shifts)
    {
        return shifts.Select(x => x.IsQuaranteeWeekend).ToArray();
    }

    private static int[] GetRequiredEmployeesNumberArr(List<Shift> shifts)
    {
        return shifts.Select(x => x.EmployeesNumber).ToArray();
    }

    private static int[,] GetWorkingField(int employeesNumber, int shiftsNumber, bool[] weekendMask, int allowedShiftsPerDay = 1)
    {
        int[,] workingField = new int[employeesNumber, shiftsNumber];
        var shiftsPerDay = shiftsNumber / (daysInWeek * weeksNumber);

        for (int shift = 0; shift < shiftsNumber; shift++)
        {
            if (!weekendMask[(shift / shiftsPerDay) % daysInWeek])
            {
                for (int i = 0; i < employeesNumber; i++)
                {
                    workingField[i, shift] = allowedShiftsPerDay;
                }
            }
        }

        return workingField;
    }

    private static GeneratingResult GenerateField(
        int[,] workingField,
        bool[] weekendMask,
        bool[] guaranteeMask,
        int[] requiredEmployeesNumberArr
        )
    {
        var shiftsNumber = guaranteeMask.Length;
        int[] exceedingWorkingShiftsArr = new int[workingField.GetLength(0)];

        for (int week = 0; week < weeksNumber; week++)
        {
            for (int day = 0; day < daysInWeek; day++)
            {
                if (!weekendMask[day])
                {
                    for (int shift = 0; shift < shiftsNumber; shift++)
                    {
                        var shiftIndex = week * daysInWeek + day * shiftsNumber + shift;
                        var requiredEmployees = requiredEmployeesNumberArr[shift];

                        if (!CheckFreeEmployees(workingField, shiftIndex, requiredEmployees))
                        {
                            return GeneratingResult.Failure;
                        }

                        for (int i = 0; i < requiredEmployees; i++)
                        {

                            var employeeIndex = FindMinExceedingIndex(workingField, shiftIndex, exceedingWorkingShiftsArr);

                            AssignShift(workingField, shiftIndex, employeeIndex, shiftsNumber);

                            if (guaranteeMask[shift])
                            {
                                AddWeekend(workingField, shiftIndex, employeeIndex, shiftsNumber);
                            }
                        }
                    }
                }
            }
        }

        return GeneratingResult.Success;
    }

    private static void AddWeekend(int[,] workingField, int shiftIndex, int employeeIndex, int shiftsNumber)
    {
        var firstIndex = shiftIndex - (shiftIndex % shiftsNumber) + shiftsNumber;
        var secondIndex = firstIndex + shiftsNumber;

        if (workingField.GetLength(1) > secondIndex)
        {
            for (int i = firstIndex; i < secondIndex; i++)
            {
                workingField[employeeIndex, i] = 0;
            }
        }
    }

    private static bool CheckFreeEmployees(int[,] workingField, int shiftIndex, int requiredEmployees)
    {
        var employeesNumber = workingField.GetLength(0);
        var freeEmpNumber = 0;

        for (int i = 0; i < employeesNumber; i++)
        {
            if (workingField[i, shiftIndex] > 0)
            {
                freeEmpNumber++;
            }
        }

        return freeEmpNumber >= requiredEmployees;
    }

    private static int FindMinExceedingIndex(int[,] workingField, int shiftIndex, int[] exceedingWorkingShifts)
    {
        int minIndex = 0;
        int minNumber = exceedingWorkingShifts[0];

        for (int i = 0; i < exceedingWorkingShifts.Length; i++)
        {
            if (workingField[i, shiftIndex] > 0)
            {
                if (exceedingWorkingShifts[i] == 0)
                {
                    minIndex = i;
                    break;
                }

                if (exceedingWorkingShifts[i] < minNumber)
                {
                    minNumber = exceedingWorkingShifts[i];
                    minIndex = i;
                }
            }
        }

        exceedingWorkingShifts[minIndex]++;
        UpdateExceeding(exceedingWorkingShifts);

        return minIndex;
    }

    private static void UpdateExceeding(int[] exceedingWorkingShifts)
    {
        var min = exceedingWorkingShifts.Min();

        for (int i = 0; i < exceedingWorkingShifts.Length; i++)
        {
            exceedingWorkingShifts[i] -= min;
        }
    }

    private static void AssignShift(int[,] workingField, int shiftIndex, int employeeIndex, int shiftsNumber)
    {
        var firstIndex = shiftIndex - (shiftIndex % shiftsNumber);
        var secondIndex = firstIndex + shiftsNumber;

        for (int i = firstIndex; i < secondIndex; i++)
        {
            if (workingField[employeeIndex, i] >= 0)
            {
                workingField[employeeIndex, i]--;
            }
        }

        workingField[employeeIndex, shiftIndex] = -1;
    }

    private static void SaveTimetable(Timetable timetable, int[,] workingField)
    {
        if (timetable.Employees != null)
        {
            for (int i = 0; i < workingField.GetLength(0); i++)
            {
                var workDays = "";
                for (int j = 0; j < workingField.GetLength(1); j++)
                {
                    if (workingField[i, j] != -1)
                    {
                        workDays += "0";
                    }
                    else
                    {
                        workDays += "1";
                    }
                }

                timetable.Employees[i].WorkDays = workDays;
            }
        }
    }
}
