using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Services;
using DC.Web.Ui.Settings.Models;
using DC_AdminQueueMonitor.Properties;
using DC_AdminQueueMonitor.Services;
using ESFA.DC.JobStatus.Interface;
using JobSubmit.Services;
using Microsoft.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DC_AdminQueueMonitor.Controllers
{
    public class QueueController : Controller
    {
        // GET: Queue
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetTopicData(int id)
        {
            string connectionString = Settings.Default.ServiceBusConnectionString;
            var nsmgr = NamespaceManager.CreateFromConnectionString(connectionString);
            var subs = nsmgr.GetSubscriptions(Settings.Default.ilrtopic);


            List<Subscription> subscriptions = new List<Subscription>();

            int i = 0;
            foreach (var sub in subs)
            {
                var activeMessages = sub.MessageCountDetails.ActiveMessageCount;
                var deadLetterMessages = sub.MessageCountDetails.DeadLetterMessageCount;
                subscriptions.Add(new Subscription()
                {
                    Name = sub.Name,
                    ActiveMessageCount = activeMessages, // + ++i,
                    DeadletterMessageCount = deadLetterMessages
                });
            }


            return Json(subscriptions);
        }

        [HttpPost]
        public async Task<ActionResult> GetJobCount(DateTime time)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sendingsite, cert, chain, sslPolicyErrors) => true;

            List<long> result = new List<long>();

            var apiurl = Settings.Default.JobBaseUrl;
            var apiSettings = new ApiSettings() { JobAPIBaseUrl = apiurl };
            var _httpClient = new BespokeHttpClient();
            var _serializationService = new ESFA.DC.Serialization.Json.JsonSerializationService();


            var _submissionService = new SubmissionService(
                new JobQueueService(apiSettings, _httpClient),
                new CloudStorageSettings()
                {
                    ConnectionString = "", //environment.StorageAccountConnectionString,
                    ContainerName = "" // Settings.Default.StorageContainer
                },
                _httpClient,
                apiSettings,
                _serializationService);
            //_collectionsManagementService = new CollectionsManagementService(_httpClient, apiSettings, _serializationService);
            //_collectionPeriods = await _collectionsManagementService.GetAllCollectionReturnCalendar();
            //_appLogsDatabase = environment.AppLogsConnectionString;
            //_auditDatabase = environment.AuditConnectionString;
            //BuildJobGrid();

            DateTime date = DateTime.UtcNow;
            TimeSpan timegap = TimeSpan.FromMinutes(5);
            var jobs = await _submissionService.GetAllJobs();
            result.Add( jobs.Where(s => (date - s.DateTimeSubmittedUtc) < timegap).Count());
            result.Add(jobs.Where(s => (date - s.DateTimeUpdatedUtc) < timegap && JobStatusService.StatusWorking(s.Status)).Count());
            result.Add(jobs.Where(s => (date - s.DateTimeUpdatedUtc) < timegap && JobStatusService.StatusFailed(s.Status)).Count());
            result.Add(jobs.Where(s => (date - s.DateTimeUpdatedUtc) < timegap && s.Status == JobStatusType.Completed).Count());
            return Json(result);
        }
    }
}