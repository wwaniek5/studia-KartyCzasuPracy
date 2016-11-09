using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkDays.Domain;
using WorkDays.Domain.Entities;
using FluentAssertions;

namespace WorkDays.Tests.DomainTests
{
    [TestFixture]
    class MonthCreatorTests
    {
        [Test]
        public void GetWorkDaysList_CorrectNumberOfDays()
        {
            MonthCreator creator = new MonthCreator();
            List<worklog> worklogs = new List<worklog>();

            var days = creator.GetWorkDays(worklogs, 2, 2016);

            Assert.AreEqual(29, days.Count());
        }

        [Test]
        public void GetWorkDaysList_WorklogPresentInInput_DayPresentInOutput()
        {
            MonthCreator creator = new MonthCreator();
            List<worklog> worklogs = new List<worklog>();
            worklogs.Add(new worklog { STARTDATE = new DateTime(2016, 1, 1) });

            var days = creator.GetWorkDays(worklogs, 1, 2015).ToList();

            Assert.AreEqual(8, days[0].StartTime.Value.Hour);
            Assert.AreEqual(16, days[0].EndTime.Value.Hour);
        }


        [Test]
        public void GetWorkDaysList_WorklogNotPresentInInput_DayNotPresentInOutput()
        {
            MonthCreator creator = new MonthCreator();
            List<worklog> worklogs = new List<worklog>();

            var days = creator.GetWorkDays(worklogs, 1, 2015).ToList();




            Assert.IsFalse( days[0].StartTime.HasValue);

        }

        [Test]
        public void GetWorkDaysList_WorklogwithNoIssue_StatusNormal()
        {
            MonthCreator creator = new MonthCreator();
            List<worklog> worklogs = new List<worklog>();
            worklogs.Add(new worklog { STARTDATE = new DateTime(2016, 1, 1) });

            var days = creator.GetWorkDays(worklogs, 1, 2015).ToList();

            Assert.AreEqual(Status.Normal, days[0].Status);
        }

        [Test]
        public void GetWorkDaysList_CorrectStatus()
        {
            MonthCreator creator = new MonthCreator();
            List<worklog> worklogs = new List<worklog>();
            worklogs.Add(new worklog { STARTDATE = new DateTime(2016, 1, 1) ,issueid=issueidCodes.Default.L4});

            var days = creator.GetWorkDays(worklogs, 1, 2015).ToList();

            Assert.AreEqual(Status.L4, days[0].Status);
        }

        //[Test]
        //public void GetWorkDaysList_GetStatus()
        //{
        //    throw new NotImplementedException();
        //}

    }
}
