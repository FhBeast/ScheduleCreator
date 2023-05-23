using Schedule.Entities;

namespace Schedule.Services;
public static class TimetableGenerator
{
    private static int daysInWeek = 7;
    private static int weeksNumber = 4;
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
        int[] exceedingWorkingShiftsArr = new int[employeesNumber];
        
        int[,] workingField = GetWorkingField(employeesNumber, shiftsNumber, weekendMask, allowedShiftsPerDayNumber);
                
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

        for (int shift = 0; shift < shiftsNumber; shift++)
        {
            if (!weekendMask[shift % daysInWeek])
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
        int[] requiredEmployeesNumberArr,
        int[] exceedingWorkingShiftsArr)
    {
        var result = GeneratingResult.Success;
        var shiftsNumber = guaranteeMask.Length;

        for (int week = 0; week < weeksNumber; week++)
        {
            for (int day = 0; day < daysInWeek; day++)
            {
                for (int shift = 0; shift < shiftsNumber; shift++)
                {

                }
            }
        }        

        return result;
    }
}
