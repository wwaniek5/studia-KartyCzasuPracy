using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using WorkDays.Domain.Exceptions;

namespace WorkDays.Domain
{
    public interface IDatabaseManager
    {
        List<worklog> GetWorklogsForAllWorkers(int month, int year);
        List<bts_projectroles> GetDetailsForAllWorkers(int month, int year);
        List<app_user> GetNameMappingsForAllWorkers();
    }

    public class JiraDatabaseManager : IDatabaseManager
    {
        private readonly IContextFactory _factory;

        public JiraDatabaseManager(IContextFactory factory)
        {
            this._factory = factory;
        }

        public List<bts_projectroles> GetDetailsForAllWorkers(int month, int year)
        {
            using (var context = _factory.Create())
            {
                var monthBeginning = new DateTime(year, month, 1);
                var nextMonthBeginning = monthBeginning.AddMonths(1);

                 
                        return context.bts_projectroles
                            .Where(v => v.date_from < nextMonthBeginning && v.date_to > monthBeginning)
                            .Where(v=>v.project==null && v.group_name==null)
                            .ToList();
            }
        }

        public List<app_user> GetNameMappingsForAllWorkers()
        {
            using (var context = _factory.Create())
            {
                return context.app_user.ToList();
            }
        }

        public List<worklog> GetWorklogsForAllWorkers(int month, int year)
        {
  
            using (var context = _factory.Create())
            {
                

                var monthBeginning = new DateTime(year, month, 1);
                var nextMonthBeginning = monthBeginning.AddMonths(1);

                //jak zmienie to się psuje
                //return context.worklogs
                //    .Where(v => v.STARTDATE >= monthBeginning && v.STARTDATE < nextMonthBeginning)
                //    .ToList();

                return context.worklogs
                   .Where(v => SqlFunctions.DatePart("year", v.STARTDATE) == SqlFunctions.DatePart("year", new DateTime(year, month, 1)))
                    .Where(v => SqlFunctions.DatePart("month", v.STARTDATE) == SqlFunctions.DatePart("month", new DateTime(year, month, 1)))
                    .ToList();
            }
        }
    }

    public interface IContextFactory
    {
        jiradbEntities Create();
    }

    public class ContextFactory : IContextFactory
    {
        public jiradbEntities Create()
        {
            return new jiradbEntities();
        }
    }
}