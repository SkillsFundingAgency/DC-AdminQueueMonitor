using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DC_AdminQueueMonitor
{
    public class QueueViewModel
    {
        public IEnumerable<Topic> Topics { get; set; }
        public JobCount LastFiveMinutes { get; set; }
        public JobCount Hour { get; set; }
        public JobCount Today { get; set; }
        public JobCount Yesterday { get; set; }
    }
}
