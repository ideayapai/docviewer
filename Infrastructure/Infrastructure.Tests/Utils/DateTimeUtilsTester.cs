using System;
using Infrasturcture.Utils;
using NUnit.Framework;

namespace Infrasturcture.Tests.Utils
{
    [TestFixture]
    public class DateTimeUtilsTester
    {
        [Test]
        public void should_get_duration()
        {
            string begin = string.Empty;
            string end = string.Empty;
            DateTimeUtils.GetDuratioin(DateTimeConstants.Today, ref begin, ref end);
            Console.WriteLine("today:");
            Console.WriteLine("begin:" + begin);
            Console.WriteLine("end:" + end);

            DateTimeUtils.GetDuratioin(DateTimeConstants.Yesterday, ref begin, ref end);
            Console.WriteLine("yesterday:");
            Console.WriteLine("begin:" + begin);
            Console.WriteLine("end:" + end);

            DateTimeUtils.GetDuratioin(DateTimeConstants.LastWeek, ref begin, ref end);
            Console.WriteLine("lastweek:");
            Console.WriteLine("begin:" + begin);
            Console.WriteLine("end:" + end);

            DateTimeUtils.GetDuratioin(DateTimeConstants.LastMonth, ref begin, ref end);
            Console.WriteLine("lastmonth:");
            Console.WriteLine("begin:" + begin);
            Console.WriteLine("end:" + end);

            DateTimeUtils.GetDuratioin(DateTimeConstants.ThisYear, ref begin, ref end);
            Console.WriteLine("thisyear:");
            Console.WriteLine("begin:" + begin);
            Console.WriteLine("end:" + end);

            DateTimeUtils.GetDuratioin(DateTimeConstants.LastYear, ref begin, ref end);
            Console.WriteLine("lastyear:");
            Console.WriteLine("begin:" + begin);
            Console.WriteLine("end:" + end);
        }
    }
}
