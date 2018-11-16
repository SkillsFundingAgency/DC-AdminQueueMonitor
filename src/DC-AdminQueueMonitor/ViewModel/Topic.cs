using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DC_AdminQueueMonitor
{
    public class Topic
    {
        public string Name { get; set; }

        public IEnumerable<Subscription> Subscriptions { get; set; }
    }
}
