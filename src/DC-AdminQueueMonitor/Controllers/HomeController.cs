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

namespace DC_AdminQueueMonitor.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            QueueViewModel result = new QueueViewModel();

            var x = System.Web.Configuration.WebConfigurationManager.AppSettings["ServiceBusConnectionString"];
            string connectionString = ConfigurationManager.AppSettings["ServiceBusConnectionString"];  //Settings.Default.ServiceBusConnectionString;

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
            JobStatusService jobStatusService = new JobStatusService(_submissionService);

            var jsc = await jobStatusService.GetStatusCounts(new List<long>() { 5, 60, 60*8 });

            result.LastFiveMinutes = jsc.ElementAt(0);
            result.Today = jsc.ElementAt(2);

            return View(result);
        }
    }
}