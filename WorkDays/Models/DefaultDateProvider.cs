using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkDays.Models
{
    public interface IDefaultDateProvider
    {
        int GetMonth();
        int GetYear();
    }

    public class DefaultDateProvider : IDefaultDateProvider
    {
        private DateTime dateTime;

        public DefaultDateProvider(DateTime dateTime)
        {
            this.dateTime = dateTime;
        }

        public int GetMonth()
        {
            int month= dateTime.Month;
            int year= dateTime.Year;
            if (month == 1)
                return 12;
            return month-1;
        }

        public int GetYear()
        {
            int month = dateTime.Month;
            int year = dateTime.Year;
            if (month == 1)
                return year-1;
            return year;
        }
    }
}