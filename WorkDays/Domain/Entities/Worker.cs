using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkDays.Domain.Entities
{
    public class Worker
    {
        private readonly string _oldName;
        private readonly string _newName;
        private readonly string _position;
        private readonly bool _isEmployed;
        private readonly string _company;
        private readonly IEnumerable<Day> _workDays;

        public Worker(string oldName, string newName, string position, bool isEmployed, string company, IEnumerable<Day> workDays)
        {
            _oldName = oldName;
            _newName = newName;
            _position = position;
            _isEmployed = isEmployed;
            _workDays = workDays;
            _company = company;
        }
        public string FormatedOldName
        {
            get { return FormatName(_oldName); }
        }

        public string FormatedNewName
        {
            get { return FormatName(_newName); }
        }

        public string NewName
        {
            get { return _newName; }
        }
        public string OldName
        {
            get { return _oldName; }
        }
        public string Position
        {
            get { return _position; }
        }
        public bool IsEmployed
        {
            get { return _isEmployed; }
        }

        public string Company
        {
            get { return _company; }
        }
        public IEnumerable<Day> WorkDays
        {
            get { return _workDays; }
        }

        private string FormatName(string workerName)
        {
            var names = workerName.Split('.');
            var fullName = "";
            foreach (string name in names)
            {
                fullName = fullName + " " + name.First().ToString().ToUpper() + String.Join("", name.Skip(1));
            }
            return fullName.Remove(0, 1);
        }

        //8 hours for every worked day
        public int GetWorkedHourForTheMonth()
        {
            int total = 0;
            foreach (Day day in WorkDays)
            {
                if (day.Status == Status.Normal)
                    total = total + 8;
            }
            return total;
        }
    }
}
