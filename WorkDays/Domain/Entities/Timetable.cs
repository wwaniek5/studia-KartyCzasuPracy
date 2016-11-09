using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkDays.Domain.Entities;

namespace WorkDays.Domain
{
    public class Timetable
    {
        public List<Day> Days { get; set; }
        public string Worker { get; set; }
        public DateTime Date { get; set; }
        public int WorkedHours { get;  set; }
        public bool Empty { get; set; }
        public string Position { get; internal set; }
        public string Company { get; internal set; }

        public Timetable()
        {
            Empty = false;
        }
        public Timetable(List<Day> days)
        {
            Days = days;
            Empty = false;
        }
    }
}
