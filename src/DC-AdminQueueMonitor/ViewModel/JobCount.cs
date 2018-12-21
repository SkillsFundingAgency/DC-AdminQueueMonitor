using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DC_AdminQueueMonitor
{
    public class JobCount
    {
        public int Submitted { get; set; }
        public int Processing { get; set; }
        public int Failed { get; set; }
        public int Completed { get; set; }
        public double AverageJobProcessingTime { get; set; }
        public double AverageQueueTime { get; set; }
    }
}