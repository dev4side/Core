using System;
using NUnit.Framework;

namespace Core.Extensions.Test
{
    [TestFixture]
    public class DateTimeEqualityComparerTest
    {
        [Test]
        public void DateTimeEqualityComparerFailsIfItIsEqual()
        {
            var baseDateTime = DateTime.Now;
            var dateTimeEqualityComparer = new DateTimeEqualityComparer(new TimeSpan(12));

            var result = dateTimeEqualityComparer.Equals(baseDateTime, baseDateTime.AddTicks(13));
            Assert.AreEqual(false, result);
        }

        [Test]
        public void DateTimeEqualityComparerFailsIfItIsNotEqual()
        {
            var baseDateTime = DateTime.Now;
            var dateTimeEqualityComparer = new DateTimeEqualityComparer(new TimeSpan(12));

            var result = dateTimeEqualityComparer.Equals(baseDateTime, baseDateTime.AddTicks(8));
            Assert.AreEqual(true, result);
        }
    }
}
