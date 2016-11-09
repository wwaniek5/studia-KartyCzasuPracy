using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkDays.Models;
using FluentAssertions;

namespace WorkDays.Tests
{
    [TestFixture]
    public class DefaultDateProviderTests
    {
        [Test]
        public void default_month_should_be_previous_to_current()
        {
            DefaultDateProvider provider = new DefaultDateProvider(new DateTime(2016, 1, 2));

            provider.GetYear().Should().Be(2015);
            provider.GetMonth().Should().Be(12);
        }
    }
}
