using System;
using System.Collections.Generic;
using System.Linq;
using WorkDays.Domain.Entities;

namespace WorkDays.Domain
{
    public interface IMonthCreator
    {
        IEnumerable<Day> GetWorkDays(List<worklog> worklogs, int month, int year);
    }

    public class MonthCreator: IMonthCreator
    {
        public IEnumerable<Day> GetWorkDays(List<worklog> worklogs, int month, int year)
        {
            for(int dayOfMonth = 1; dayOfMonth <= DateTime.DaysInMonth(year, month); dayOfMonth++)
            {
                var day = new Day
                {
                    DayOfMonth = dayOfMonth
                };

                var dayWorklog = TryGetDayWorklog(worklogs, dayOfMonth);

                if (dayWorklog != null)
                {
                    day.Status = GetStatus(dayWorklog);
                    day.StartTime = new DateTime(year, month, dayOfMonth, 8, 0, 0);
                    day.EndTime = new DateTime(year, month, dayOfMonth, 16, 0, 0);
                }else
                {
                    day.Status = Status.Free;
                }

                yield return day;
            }
        }

        private worklog TryGetDayWorklog(List<worklog> worklogs, int dayOfMonth)
        {
            return worklogs
                .Where(worklog => worklog.STARTDATE.HasValue)
                .FirstOrDefault(worklog => worklog.STARTDATE.Value.Day == dayOfMonth);
        }

        private Status GetStatus(worklog log)
        {
            if (log.issueid.HasValue)
            {
                if (((int)log.issueid.Value) == issueidCodes.Default.Urlop)
                {
                    return Status.Urlop;
                }else if(((int)log.issueid.Value) == issueidCodes.Default.UrlopDodatkowy)
                {
                    return Status.UrlopDodatkowy;
                }
                else if (((int)log.issueid.Value) == issueidCodes.Default.L4)
                {
                    return Status.L4;
                }
            }
            return Status.Normal;
        }
    }
}