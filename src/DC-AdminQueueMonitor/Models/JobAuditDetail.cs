using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DC_AdminQueueMonitor.Models
{
    public class JobAuditDetail
    {
        public List<JobTask> Tasks { get; set; }
        public List<TimeSpan> Durations { get; set; }
        public DateTime JobSubmittedTime { get; set; }
        public TimeSpan JobQueueTime { get; set; }
    }
}