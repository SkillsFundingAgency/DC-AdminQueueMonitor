using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DC_AdminQueueMonitor
{
    public class Subscription
    {
        public string Name { get; set; }

        public long ActiveMessageCount { get; set; }

        public long DeadletterMessageCount { get; set; }
    }
}
