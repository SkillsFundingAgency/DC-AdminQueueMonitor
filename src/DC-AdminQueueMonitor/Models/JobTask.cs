using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DC_AdminQueueMonitor.Models
{
    public class JobTask
    {
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
    }
}