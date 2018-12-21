using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DC_AdminQueueMonitor.Models
{
    public class DateRangeUtc
    {
        public DateTime FromUtc { get; set; }
        public DateTime ToUtc { get; set; }

        internal static DateRangeUtc FromTimeSpan(TimeSpan timeSpan)
        {
            return new DateRangeUtc()
            {
                ToUtc = DateTime.UtcNow,
                FromUtc = DateTime.UtcNow - timeSpan
            };
        }

        internal static DateRangeUtc FromDateToDay(DateTime whichDay)
        {
            return new DateRangeUtc()
            {
                FromUtc = new DateTime(whichDay.Year,whichDay.Month,whichDay.Day,0,0,0,0,DateTimeKind.Utc),
                ToUtc = new DateTime(whichDay.Year, whichDay.Month, whichDay.Day, 23, 59, 59, 99, DateTimeKind.Utc)
            };
        }
    }
}