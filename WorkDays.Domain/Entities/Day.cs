using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkDays.Domain.Entities
{
    public enum Status { Normal, Urlop, UrlopDodatkowy, L4, Free, Empty };
    public class Day
    {
        public int DayOfMonth;
        public DateTime? StartTime;
        public DateTime? EndTime;    
        public Status Status;


        //TODO Tak czy zmienić nazwy enum?
        public static string ConvertStatusToAbreviation(Status status)
        {
            switch (status)
            {
                case WorkDays.Domain.Entities.Status.Free:
                    return ("-");
                case WorkDays.Domain.Entities.Status.Normal:
                    return ("8");
                case WorkDays.Domain.Entities.Status.Urlop:
                    return ("Uw");
                case WorkDays.Domain.Entities.Status.UrlopDodatkowy:
                    return ("Uok");
                case WorkDays.Domain.Entities.Status.L4:
                    return ("Ch");
                case WorkDays.Domain.Entities.Status.Empty:
                    return ("");

            }
            throw new ArgumentOutOfRangeException("No such status");
        }
    }
}
