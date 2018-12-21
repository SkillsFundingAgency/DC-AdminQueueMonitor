using Microsoft.ServiceBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace DC_AdminQueueMonitor.Services
{
    public class ServiceBusService
    {
        private string _connectionString;

        public ServiceBusService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Topic> GetTopicsAndSubscriptions()
        {
            var nsmgr = NamespaceManager.CreateFromConnectionString(_connectionString);
            var topics = nsmgr.GetTopics();
            List<Topic> result = new List<Topic>();
            foreach (var topic in topics)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Topic t = new Topic()
                {
                    Name = topic.Path
                };
                var subs = nsmgr.GetSubscriptions(t.Name);
                List<Subscription> subscriptions = new List<Subscription>();

                foreach (var sub in subs)
                {
                    var activeMessages = sub.MessageCountDetails.ActiveMessageCount;
                    var deadLetterMessages = sub.MessageCountDetails.DeadLetterMessageCount;
                    subscriptions.Add(new Subscription()
                    {
                        Name = sub.Name,
                        ActiveMessageCount = activeMessages,
                        DeadletterMessageCount = deadLetterMessages
                    });
                }

                t.Subscriptions = subscriptions;
                t.Duration = sw.Elapsed;
                result.Add(t);
            }
            return result;
        }
    }
}