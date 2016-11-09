using Moq;
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
    class TimetablesProviderTests
    {
        public class Fixture
        {
            private readonly List<worklog> _worklogs = new List<worklog>();
            private readonly List<bts_projectroles> bts_projectrolesList = new List<bts_projectroles>();
            private readonly List<app_user> _nameMappings = new List<app_user>();

            public Fixture AddWorklog(worklog worklog)
            {
                _worklogs.Add(worklog);
                return this;
            }

            public ITimetableProvider Build()
            {
                var databaseManagerMock = new Mock<IDatabaseManager>();
                databaseManagerMock.Setup(m => m.GetWorklogsForAllWorkers(It.IsAny<int>(), It.IsAny<int>())).Returns(_worklogs);
                databaseManagerMock.Setup(m => m.GetDetailsForAllWorkers(It.IsAny<int>(), It.IsAny<int>())).Returns(bts_projectrolesList);
                databaseManagerMock.Setup(m => m.GetNameMappingsForAllWorkers()).Returns(_nameMappings);

                return new TimetableProvider(new WorkersFactory(databaseManagerMock.Object,  new MonthCreator()));
            }

            internal Fixture AddDetail(bts_projectroles bts_projectroles)
            {
                bts_projectrolesList.Add(bts_projectroles);
                return this;

            }

            internal Fixture AddNameMapping(app_user app_user)
            {
                _nameMappings.Add(app_user);
                return this;
            }
        }

        [Test]
        public void translates_from_old_to_new_names()
        {
            var fixture = new Fixture();

            fixture
                .AddWorklog(new worklog { AUTHOR = "panienskie", STARTDATE = new DateTime(2016, 1, 1) });
            fixture
                .AddDetail(new bts_projectroles { name = "panienskie", role_agreement = "zamiatacz" });
            fixture
                .AddNameMapping(new app_user { user_key = "panienskie", lower_user_name = "po_malzonku" });

            var sut = fixture.Build();
            var result = sut.GetTimetableForSpecificWorker("po_malzonku", 1, 2016);

            result.Worker.ShouldBeEquivalentTo("Po_malzonku");
        }

        [Test]
        public void can_get_timetable_for_specifc_worker()
        {
            var fixture = new Fixture();

            fixture
                .AddWorklog(new worklog { AUTHOR = "A" ,STARTDATE=new DateTime(2016,1,1)})//will be included
                .AddWorklog(new worklog { AUTHOR = "A", STARTDATE = new DateTime(2016, 1, 2) })//will be included
                .AddWorklog(new worklog { AUTHOR = "B" });//will NOT be included

            fixture
                .AddDetail(new bts_projectroles { name = "A" , role_agreement ="zamiatacz"})
                .AddDetail(new bts_projectroles { name = "B" });


            var sut = fixture.Build();

            var result = sut.GetTimetableForSpecificWorker("A", 1, 2016);

            result.Position.ShouldBeEquivalentTo("zamiatacz");
            result.Days.ShouldBeEquivalentTo(GetExpectedDaysFora());
        }


        [Test]
        public void can_get_all_workers()
        {
            var fixture = new Fixture();

            fixture
                .AddWorklog(new worklog { AUTHOR = "A" })
                .AddWorklog(new worklog { AUTHOR = "A" })
                .AddWorklog(new worklog { AUTHOR = "B" });

            fixture
                .AddDetail(new bts_projectroles { name = "A" })
                .AddDetail(new bts_projectroles { name = "B" });

            var sut = fixture.Build();

            var result = sut.GetTimetableForAllWorkers(1, 2016);

            result.Select(x => x.Worker).ShouldBeEquivalentTo(new[]
           {
                "A", "B"
            });
        }

        [Test]
        public void can_get_all_employed_workers()
        {
            var fixture = new Fixture();

            fixture
                .AddWorklog(new worklog { AUTHOR = "A" })
                .AddWorklog(new worklog { AUTHOR = "A" })
                .AddWorklog(new worklog { AUTHOR = "B" })
                .AddWorklog(new worklog { AUTHOR = "C" });

            fixture
                .AddDetail(new bts_projectroles { name = "A" })
                .AddDetail(new bts_projectroles { name = "B", agreement_type ="emp"})
                .AddDetail(new bts_projectroles { name = "C", agreement_type = "somethingelse" });

            var sut = fixture.Build();

            var result = sut.GetTimetableForAllEmployedWorkers(1, 2016);

            result.Select(x => x.Worker).ShouldBeEquivalentTo(new[]
           {
                "A", "B"
            });
        }

        [Test]
        public void can_get_all_employed_workers_even_if_worker_not_present_in_bts_projectroles()
        {
            var fixture = new Fixture();

            fixture
                .AddWorklog(new worklog { AUTHOR = "A" })
                .AddWorklog(new worklog { AUTHOR = "A" })
                .AddWorklog(new worklog { AUTHOR = "B" })
                .AddWorklog(new worklog { AUTHOR = "C" });

            fixture
                .AddDetail(new bts_projectroles { name = "A" })
                .AddDetail(new bts_projectroles { name = "B", agreement_type = "emp" });

            var sut = fixture.Build();

            var result = sut.GetTimetableForAllEmployedWorkers(1, 2016);

            result.Select(x => x.Worker).ShouldBeEquivalentTo(new[]
           {
                "A", "B","C"
            });
        }

        //[Test]
        //public void can_give_correct_positions_depending_on_month()
        //{
        //    var fixture = new Fixture();

        //    fixture
        //        .AddWorklog(new worklog { AUTHOR = "A" });

        //    fixture
        //        .AddDetail(new bts_projectroles { name = "A" ,date_from=new DateTime(2014,1,1), date_to = new DateTime(2014, 5, 31) ,role_agreement="zamiatacz"})
        //        .AddDetail(new bts_projectroles { name = "A", date_from = new DateTime(2014, 6, 1), date_to = new DateTime(2100, 6, 1), role_agreement = "hutnik" });

        //    var sut = fixture.Build();

        //    var firstPeriod = sut.GetTimetableForSpecificWorker("A", 5, 2014);
        //    firstPeriod.Position.ShouldBeEquivalentTo("zamiatacz");

        //    var secondPeriod = sut.GetTimetableForSpecificWorker("A", 6, 2014);
        //    firstPeriod.Position.ShouldBeEquivalentTo("hutnik");
        //}

        [Test]
        public void can_get_correct_days_for_all_workers()
        {
            var fixture = new Fixture();

            fixture
               .AddWorklog(new worklog { AUTHOR = "A", STARTDATE = new DateTime(2016, 1, 1) })//will be included
               .AddWorklog(new worklog { AUTHOR = "A", STARTDATE = new DateTime(2016, 1, 2) })//will be included
               .AddWorklog(new worklog { AUTHOR = "B", STARTDATE = new DateTime(2016, 1, 1) });//will be be included

            fixture
                .AddDetail(new bts_projectroles { name = "A" })
                .AddDetail(new bts_projectroles { name = "B" });

            var sut = fixture.Build();

            var result = sut.GetTimetableForAllWorkers(1, 2016);

            var r = result.Select(x => x.Days);

            r.ShouldBeEquivalentTo(new[]
           {
                GetExpectedDaysFora(),  GetExpectedDaysForb()
            });
        }

        private object GetExpectedDaysForb()
        {
            List<Day> days = GetDefaultDays();
           

            days[0].Status = Status.Normal;
            days[0].StartTime = new DateTime(2016, 1, 1, 8, 0, 0);
            days[0].EndTime = new DateTime(2016, 1, 1, 16, 0, 0);

            return days;
        }

        private List<Day> GetDefaultDays()
        {
            List<Day> days = new List<Day>();
            for (int i = 1; i <= 31; i++)
                days.Add(new Day { DayOfMonth = i, Status = Status.Free });
            return days;
        }

        private object GetExpectedDaysFora()
        {
            List<Day> days = GetDefaultDays();

            days[0].Status = Status.Normal;
            days[0].StartTime = new DateTime(2016, 1, 1, 8, 0, 0);
            days[0].EndTime = new DateTime(2016, 1, 1, 16, 0, 0);

            days[1].Status = Status.Normal;
            days[1].StartTime = new DateTime(2016, 1, 2, 8, 0, 0);
            days[1].EndTime = new DateTime(2016, 1, 2, 16, 0, 0);

            return days;
        }



        //[Test]
        //public void GetTableModels_NoUser_ReturnsForAllUsers()
        //{
        //    var MockManager = new Mock<IDatabaseManager>();
        //    List<worklog> worklogs = new List<worklog> { new worklog { AUTHOR = "a" }, new worklog { AUTHOR = "b" }, new worklog { AUTHOR = "b" } };
        //    MockManager.Setup(m => m.GetWorklogsForAllWorkers(It.IsAny<int>(), It.IsAny<int>())).Returns(worklogs);

        //    var MockMonthCreator = new Mock<IMonthCreator>();
        //    MockMonthCreator.Setup(m => m.GetWorkDays(It.IsAny<List<worklog>>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Day>());

        //    TimetableProvider provider = new TimetableProvider(MockManager.Object, MockMonthCreator.Object);
        //    var result = provider.GetTimetableForAllWorkers(1, 2016);

        //    Assert.AreEqual(2, result.Count());
        //    StringAssert.AreEqualIgnoringCase("a", result[0].Author);
        //    StringAssert.AreEqualIgnoringCase("b", result[1].Author);
        //}

        //public void GetTableModels_SpecifiedUser_ReturnsForOneUsers()
        //{
        //    var mockManager = new Mock<IDatabaseManager>();
        //    List<worklog> worklogs = new List<worklog> {  new worklog { AUTHOR = "a" }, new worklog { AUTHOR = "a" } };
        //    mockManager.Setup(m => m.GetWorklogsForAllWorkers(It.IsAny<int>(), It.IsAny<int>())).Returns(worklogs);

        //    var MockMonthCreator = new Mock<IMonthCreator>();
        //    MockMonthCreator.Setup(m => m.GetWorkDays(It.IsAny<List<worklog>>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Day>());

        //    TimetableProvider provider = new TimetableProvider(mockManager.Object, MockMonthCreator.Object);
        //    var result = provider.GetTimetableForSpecificWorker("a",1, 2016);

        //    StringAssert.AreEqualIgnoringCase("a", result[0].Author);
        //    mockManager.Verify(m => m.GetWorklogsForSpecifcWorker("a", 1, 2016));
        //}


    }
}
