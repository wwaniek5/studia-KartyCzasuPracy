using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WorkDays.Controllers;
using WorkDays.Domain;
using WorkDays.Domain.Entities;
using WorkDays.Models;

namespace WorkDays.Tests.WebUITests
{
    [TestFixture]
    class HomeControllerTests
    {
        class Fixture
        {
            public HomeController GetMockController()
            {
                var providerMock = new Mock<ITimetableProvider>();
                providerMock.Setup(m => m.GetTimetableForSpecificWorker(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new Timetable(new List<Day>()));
                providerMock.Setup(m => m.GetTimetableForAllWorkers(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Timetable> { new Timetable(new List<Day>()) });

                var excelMock = new Mock<IExcelGenerator>();
                excelMock.Setup(m => m.GenerateStreamFromTemplate(It.IsAny<List<Timetable>>())).Returns(new MemoryStream());

                return new HomeController(providerMock.Object, new DefaultDateProvider(new DateTime(2000, 1, 1)), excelMock.Object);
            }

            internal UserDateModel GetModel()
            {
                var model = new UserDateModel();
                model.SelectedMonth = 1;
                model.UserName = "d";
                model.SelectedYear = 2;
                model.AllUsersSelected = false;

                return model;
            }
        }

        [Test]
        public void returns_index_when_pokaz_clicked()
        {
            Fixture fixture = new Fixture();
            HomeController homeController = fixture.GetMockController();
            UserDateModel model = fixture.GetModel();
        
            ActionResult result = homeController.Index("", null, null, model);

            Assert.IsInstanceOf<ViewResult>(result);
            ((ViewResult) result).ViewName.Should().Be("");
        }

        [Test]
        public void returns_print_when_drukuj_clicked()
        {
            Fixture fixture = new Fixture();
            HomeController homeController = fixture.GetMockController();
            UserDateModel model = fixture.GetModel();

            ActionResult result = homeController.Index(null, "", null, model);

            Assert.IsInstanceOf<ViewResult>(result);
            ((ViewResult)result).ViewName.Should().Be("Print");
        }

        [Test]
        public void returns_file_when_excel_clicked()
        {
            Fixture fixture = new Fixture();
            HomeController homeController = fixture.GetMockController();
            UserDateModel model = fixture.GetModel();

            ActionResult result = homeController.Index(null, null,"", model);

            Assert.IsInstanceOf<FileStreamResult>(result);
        }

        [Test]
        public void returns_index_when_model_not_valid()
        {
            Fixture fixture = new Fixture();
            HomeController homeController = fixture.GetMockController();
            var model = new UserDateModel();

            ActionResult result = homeController.Index(null, null,"",  model);

            Assert.IsInstanceOf<ViewResult>(result);
            ((ViewResult)result).ViewName.Should().Be("");
        }
    }
}
