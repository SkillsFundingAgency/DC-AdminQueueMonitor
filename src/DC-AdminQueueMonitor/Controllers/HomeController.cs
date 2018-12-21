using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Services;
using DC.Web.Ui.Settings.Models;
using DC_AdminQueueMonitor.Properties;
using DC_AdminQueueMonitor.Services;
using DC_AdminQueueMonitor.Models;

namespace DC_AdminQueueMonitor.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            QueueViewModel result = new QueueViewModel();

            var x = System.Web.Configuration.WebConfigurationManager.AppSettings["ServiceBusConnectionString"];
            string connectionString = ConfigurationManager.AppSettings["ServiceBusConnectionString"];  //Settings.Default.ServiceBusConnectionString;
            string auditConnectionString = ConfigurationManager.AppSettings["AuditConnectionString"];  //Settings.Default.ServiceBusConnectionString;

            ServiceBusService sbservice = new ServiceBusService(connectionString);
            result.Topics = sbservice.GetTopicsAndSubscriptions();

            var apiurl = System.Configuration.ConfigurationManager.AppSettings["JobBaseUrl"];  //Settings.Default.JobBaseUrl;
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
            var auditService = new AuditService(auditConnectionString);

            JobStatusService jobStatusService = new JobStatusService(_submissionService, auditService);

            var jsc = await jobStatusService.GetStatusCounts( 
                new List<DateRangeUtc> {
                    DateRangeUtc.FromTimeSpan(TimeSpan.FromMinutes(5)),
                    DateRangeUtc.FromTimeSpan(TimeSpan.FromHours(1)),
                    DateRangeUtc.FromDateToDay(DateTime.Today),
                    DateRangeUtc.FromDateToDay(DateTime.Today.AddDays(-1)),
                });

            result.LastFiveMinutes = jsc.ElementAt(0);
            result.Hour = jsc.ElementAt(1);
            result.Today = jsc.ElementAt(2);
            result.Yesterday = jsc.ElementAt(3);

            return View(result);
        }
    }
}