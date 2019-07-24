using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Services;
using DC.Web.Ui.Settings.Models;
using DC_AdminQueueMonitor.Models;
using ESFA.DC.Jobs.Model.Enums;
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
        private AuditService _auditService;
        public JobStatusService(SubmissionService submissionService,AuditService auditService)
        {
            _submissionService = submissionService;
            _auditService = auditService;
        }

        public async Task<IEnumerable<JobCount>> GetStatusCounts(IEnumerable<DateRangeUtc> toFroms)
        {
            DateTime date = DateTime.UtcNow;
            ServicePointManager.ServerCertificateValidationCallback += (sendingsite, cert, chain, sslPolicyErrors) => true;

            var jobs = await _submissionService.GetAllJobs();
            var result = new List<JobCount>(toFroms.Count());
            foreach (var tf in toFroms)
            {
                var jc = new JobCount();                

                jc.Submitted = jobs.Where(s => s.DateTimeSubmittedUtc >= tf.FromUtc && s.DateTimeSubmittedUtc < tf.ToUtc).Count();
                jc.Processing = jobs.Where(s => (s.DateTimeUpdatedUtc>= tf.FromUtc && s.DateTimeUpdatedUtc < tf.ToUtc) && StatusWorking(s.Status)).Count();
                jc.Failed = jobs.Where(s => (s.DateTimeUpdatedUtc >= tf.FromUtc && s.DateTimeUpdatedUtc < tf.ToUtc) && StatusFailed(s.Status)).Count();
                var completedJobs = jobs.Where(s => s.DateTimeUpdatedUtc >= tf.FromUtc && s.DateTimeUpdatedUtc < tf.ToUtc && s.Status == JobStatusType.Completed);
                jc.Completed = completedJobs.Count();
                jc.AverageJobProcessingTime = 0;
                if (jc.Completed > 0)
                {
                    jc.AverageJobProcessingTime = completedJobs.Average(s => (s.DateTimeUpdatedUtc - s.DateTimeSubmittedUtc).Value.TotalSeconds);
                    List<JobAuditDetail> jad = new List<JobAuditDetail>(jc.Completed);
                    TimeSpan time = TimeSpan.Zero;
                    TimeSpan queueTime = TimeSpan.Zero;
                    foreach ( var job in completedJobs )
                    {
                        var rc = await _auditService.GetAuditDataBlock(job.JobId);
                        time += rc.Durations.Last();
                        queueTime += rc.JobQueueTime;
                    }
                    jc.AverageJobProcessingTime = time.TotalSeconds / (float)jc.Completed / 60;
                    jc.AverageQueueTime = queueTime.TotalSeconds / (float)jc.Completed / 60;
                }
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