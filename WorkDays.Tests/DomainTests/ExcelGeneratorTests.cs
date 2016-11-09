using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using WorkDays.Domain;
using WorkDays.Domain.Entities;

namespace WorkDays.Tests.DomainTests
{
    [TestFixture]
    class ExcelGeneratorTests
    {
        [Test]
        public void returns_nonempty_stream()
        {
            Timetable timetable = new Timetable(new List<Day>());
            timetable.Worker = "a";
            ExcelGenerator generator = new ExcelGenerator();

            Stream stream=generator.GenerateStreamFromTemplate(new List<Timetable> { timetable });
            stream.Length.Should().BeGreaterThan(0);

        }

    }
}
