using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkDays.Domain;

namespace WorkDays.Models
{
    public class UserDateModel
    {
        public List<Timetable> Timetables { get; set; }

        public bool AllUsersSelected { get; set; }
        public bool AllEmployedUsersSelected { get; set; }

        // [Required(ErrorMessage = "Wpisz login")]
        public string UserName{ get; set; }

        [Required(ErrorMessage = "wybierz miesiąc")]
        public int SelectedMonth { get; set; }

        [Required(ErrorMessage = "wpisz rok")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> Months
        {
            get
            {
                string[] names = DateTimeFormatInfo.CurrentInfo.MonthNames;
                return DateTimeFormatInfo
                       .CurrentInfo
                       .MonthNames
                       .Take(12)
                       .Select((monthName, index) => new SelectListItem
                       {
                           Value = (index +1).ToString(),
                           Text = monthName
                       });
            }
        }

        private IDefaultDateProvider defaultDateProvider;

        public UserDateModel()
        {

        }

        public UserDateModel(IDefaultDateProvider provider)
        {
            Timetables = new List<Timetable>();
            defaultDateProvider = provider;
            SelectedMonth = defaultDateProvider.GetMonth();
            SelectedYear = defaultDateProvider.GetYear();
            AllUsersSelected = false;
            AllEmployedUsersSelected = true;
        }


    }
}