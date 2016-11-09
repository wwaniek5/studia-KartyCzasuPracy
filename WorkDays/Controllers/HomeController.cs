using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkDays.Domain;
using WorkDays.Domain.Entities;
using WorkDays.Domain.Exceptions;
using WorkDays.Models;

namespace WorkDays.Controllers
{
    public class HomeController : Controller
    {
        private ITimetableProvider provider;
        private IDefaultDateProvider dateProvider;
        private IExcelGenerator excelGenerator;

        public HomeController(ITimetableProvider provider, IDefaultDateProvider dateProvider, IExcelGenerator excelGenerator)
        {
            this.provider = provider;
            this.dateProvider = dateProvider;
            this.excelGenerator = excelGenerator;
        }
        // GET: Home
        public ViewResult Index()
        {
            UserDateModel model = new UserDateModel(dateProvider);
          
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string ShowButton,string PrintButton, string ExcelButton, UserDateModel model)
        {
            CheckInitialOptions(model);

            if (!ModelState.IsValid)
                return View(model);
            

            try
            {
                               
                GenerateTimetable(model);
               
            }
            catch (NoWorkerException)
            {
                ModelState.AddModelError("", "Nie ma takiego pracownika");
                return View(model);
            }


            if (PrintButton != null)
                return View("Print", model.Timetables);

            if (ExcelButton != null)
                return GenerateFileForDownload(model);
            
              

            return View(model);
        }

        private FileStreamResult GenerateFileForDownload(UserDateModel model)
        {
            Stream stream = excelGenerator.GenerateStreamFromTemplate(model.Timetables);

            FileStreamResult result= new FileStreamResult(stream, System.Net.Mime.MediaTypeNames.Application.Octet);
            result.FileDownloadName = "Karty_czasu_pracy.xlsx";

            return result;
        }

        private void GenerateTimetable(UserDateModel model)
        {
            if (model.AllUsersSelected)
            {
                GenerateTimetableForAllUsers(model);
            }
            else if(model.AllEmployedUsersSelected)
            {
                GenerateTimetableForAllEmployedUsers(model);
            }
            else
            {
                GenerateTimetableForSpecifcUser(model);
            }
        }

        private void GenerateTimetableForAllEmployedUsers(UserDateModel model)
        {
            model.Timetables = provider.GetTimetableForAllEmployedWorkers(model.SelectedMonth, model.SelectedYear);
        }

        private void GenerateTimetableForAllUsers(UserDateModel model)
        {
            model.Timetables = provider.GetTimetableForAllWorkers(model.SelectedMonth, model.SelectedYear);
        }

        private void GenerateTimetableForSpecifcUser(UserDateModel model)
        {
            model.Timetables = new List<Timetable> { provider.GetTimetableForSpecificWorker(model.UserName, model.SelectedMonth, model.SelectedYear) };
        }

        private void CheckInitialOptions(UserDateModel model)
        {
            if (model.AllUsersSelected == false && model.AllEmployedUsersSelected == false && model.UserName == null)
            {
                ModelState.AddModelError("", "Wpisz login");
            }
        }

        public ViewResult PrintEmpty()
        {
            var model = new List<Timetable> { GetEmptyTimetable() };
            return View("Print",model);
        }

        private Timetable GetEmptyTimetable()
        {
            List<Day> days = new List<Day>();
            for (int i = 1; i <= 31; i++)
            {
                Day day = new Day();
                day.DayOfMonth = i;
                day.Status = Status.Empty;
                days.Add(day);
            }

            Timetable model = new Timetable(days);
            model.Empty = true;
            return model;
        }
    }
}