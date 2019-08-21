using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Services;
using DC.Web.Ui.Settings.Models;
using DC_AdminQueueMonitor.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DC_AdminQueueMonitor.Controllers
{
    [Authorize]
    public class BusController : Controller
    {
        // GET: Bus
        public ActionResult Index()
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

            return View(result);
        }
    }
}