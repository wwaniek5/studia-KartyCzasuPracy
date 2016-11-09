using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkDays.Domain;
using WorkDays.Domain.Entities;
using WorkDays.Domain.Exceptions;

namespace WorkDays.Domain
{
    public interface ITimetableProvider
    {
        List<Timetable> GetTimetableForAllWorkers(int month, int year);
        List<Timetable> GetTimetableForAllEmployedWorkers(int month, int year);
        Timetable GetTimetableForSpecificWorker(string userName, int month, int year);
    }

    public class TimetableProvider: ITimetableProvider
    {
        private WorkersFactory workersFactory;

        public TimetableProvider(WorkersFactory workersFactory)
        {
            this.workersFactory = workersFactory;


        }

        public Timetable GetTimetableForSpecificWorker(string workerNewName, int month, int year)
        {
            List<Worker> workers = workersFactory.Create(month, year);
            var worker = workers.Where(w => w.NewName.Equals(workerNewName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (worker == null)
                throw new NoWorkerException();
            return PrepareTimetableForSpecificWorker(worker, month, year);
        }



        public List<Timetable> GetTimetableForAllWorkers(int month, int year)
        {
            List<Worker> workers = workersFactory.Create(month, year);

            List<Timetable> timetables = new List<Timetable>();
            foreach (Worker worker in workers)
            {
                timetables.Add(PrepareTimetableForSpecificWorker(worker,month,year));
            }

            return timetables;
        }

        public List<Timetable> GetTimetableForAllEmployedWorkers(int month, int year)
        {
            List<Worker> workers = workersFactory.Create(month, year);

            List<Timetable> timetables = new List<Timetable>();
            foreach (Worker worker in workers)
            {
                if(worker.IsEmployed)
                     timetables.Add(PrepareTimetableForSpecificWorker(worker, month, year));
            }

            return timetables;
        }


        private Timetable PrepareTimetableForSpecificWorker(Worker worker, int month,int year)
        {
            Timetable timeTable = new Timetable();

            timeTable.Worker = worker.FormatedNewName;
            timeTable.Position = worker.Position;
            timeTable.Company = worker.Company;
            timeTable.WorkedHours = worker.GetWorkedHourForTheMonth();
            timeTable.Days = worker.WorkDays.ToList();
            timeTable.Date = new DateTime(year, month, 1);

            return timeTable;
        }  
    }
}