using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Services;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.JobStatus.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace DC_AdminQueueMonitor.Services
{
    public class JobStatusService
    {

        private SubmissionService _submissionService;
        public JobStatusService(SubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        public async Task<IEnumerable<JobCount>> GetStatusCounts(IEnumerable<long> lastHowManyMinutes)
        {
            DateTime date = DateTime.UtcNow;
            ServicePointManager.ServerCertificateValidationCallback += (sendingsite, cert, chain, sslPolicyErrors) => true;

            var jobs = await _submissionService.GetAllJobs();
            var result = new List<JobCount>(lastHowManyMinutes.Count());
            foreach (long lhmm in lastHowManyMinutes)
            {
                TimeSpan timegap = TimeSpan.FromMinutes(lhmm);
                var jc = new JobCount();
                jc.Submitted = jobs.Where(s => (date - s.DateTimeSubmittedUtc) < timegap).Count();
                jc.Processing = jobs.Where(s => (date - s.DateTimeUpdatedUtc) < timegap && StatusWorking(s.Status)).Count();
                jc.Failed = jobs.Where(s => (date - s.DateTimeUpdatedUtc) < timegap && StatusFailed(s.Status)).Count();
                jc.Completed = jobs.Where(s => (date - s.DateTimeUpdatedUtc) < timegap && s.Status == JobStatusType.Completed).Count();
                result.Add(jc);
            }
            return result;
        }

        public static bool StatusWorking(JobStatusType status)
        {
            switch (status)
            {
                case JobStatusType.MovedForProcessing:
                case JobStatusType.Processing:
                    return true;
                default:
                    return false;
            }
        }

        public static bool StatusFailed(JobStatusType status)
        {
            switch (status)
            {
                case JobStatusType.Failed:
                case JobStatusType.FailedRetry:
                    return true;
                default:
                    return false;
            }
        }

    }
}