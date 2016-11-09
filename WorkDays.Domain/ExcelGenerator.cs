using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WorkDays.Domain.Entities;
using WorkDays.Domain.Properties;

namespace WorkDays.Domain
{
    public interface IExcelGenerator
    {
        Stream GenerateStreamFromTemplate(List<Timetable> timetables);
    }
    public class ExcelGenerator : IExcelGenerator
    {
        public Stream GenerateStreamFromTemplate(List<Timetable> timetables)
        {           
            using (ExcelPackage excelPackage = new ExcelPackage(GetStreamForTheNewFile(), GetStreamFromTheTemplate()))
            {

                foreach (Timetable timetable in timetables)
                {
                    AddSpreadsheet(excelPackage.Workbook, timetable);
                }

                SetTitle(excelPackage.Workbook); 
                excelPackage.Save();

                return ConfigureStream(excelPackage.Stream);
            }
        }

        private void SetTitle(ExcelWorkbook workbook)
        {
           workbook.Properties.Title = "Karta czasu pracy";
        }

        private MemoryStream GetStreamForTheNewFile()
        {
            return new MemoryStream();
        }

        private MemoryStream GetStreamFromTheTemplate()
        {
            MemoryStream templateStream = new MemoryStream();
            templateStream.Write(Resources.KartaCzasuPracy, 0, Resources.KartaCzasuPracy.Length);
            return templateStream;
        }

        private Stream ConfigureStream(Stream stream)
        {
            stream.Flush();
            stream.Position = 0;
            return stream;
        }

        bool firstIteration = true;

        private void AddSpreadsheet(ExcelWorkbook workbook, Timetable timetable)
        {
            string sheetName = timetable.Worker;
            ExcelWorksheet worksheet;

          
            if (firstIteration)
            {
                firstIteration = false;
                worksheet = workbook.Worksheets[1];
                worksheet.Name = sheetName;
            }
            else
            {
                worksheet = workbook.Worksheets.Add(sheetName, workbook.Worksheets[1]);
            }


            LoadTimetableDataToSheet(worksheet,timetable);
        }

        private void LoadTimetableDataToSheet(ExcelWorksheet worksheet,Timetable timetable)
        {
            worksheet.Cells["B1"].Value = timetable.Worker;
            worksheet.Cells["B2"].Value = timetable.Position;
            worksheet.Cells["B3"].Value = timetable.Company;
            worksheet.Cells["B4"].Value = timetable.Date.ToString("MMMM yyyy");

            foreach(Day day in timetable.Days)
            {
                string dayCell=GetDayCellForSpecificDay(day);
                string statusCell = GetStatusCellForSpecificDay(day);

                worksheet.Cells[dayCell].Value = day.DayOfMonth;
                worksheet.Cells[statusCell].Value = Day.ConvertStatusToAbreviation(day.Status);

            }

            worksheet.Cells["B25"].Value = timetable.WorkedHours;
        }

        private string GetStatusCellForSpecificDay(Day day)
        {
            string statusCell;

            if (day.DayOfMonth <= 16)
            {
                statusCell = "B" + (day.DayOfMonth + 7).ToString();
            }
            else
            {
                statusCell = "D" + (day.DayOfMonth + 7 - 16).ToString();
            }
            return statusCell;
        }

        private string GetDayCellForSpecificDay(Day day)
        {

            string dayCell ;
           
            if (day.DayOfMonth <= 16)
            {
                dayCell = "A" + (day.DayOfMonth + 7).ToString();
            }
            else
            {
                dayCell = "C" + (day.DayOfMonth + 7 - 16).ToString();
            }
            return dayCell;
        }
    }
}
