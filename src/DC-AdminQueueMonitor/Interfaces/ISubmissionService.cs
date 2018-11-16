using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Interface;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface ISubmissionService
    {
        Task<CloudBlobStream> GetBlobStream(string fileName);

        Task<long> SubmitIlrJob(ESFA.DC.Jobs.Model.Enums.JobType jobType, string fileName, decimal fileSizeBytes, string submittedBy, long ukprn, string collectionName, int period, bool isFirstStageOnly, string emailAddress);

        Task<FileUploadJob> GetJob(long ukprn, long jobId);

        Task<IEnumerable<FileUploadJob>> GetAllJobs(long ukprn);

        Task<string> UpdateJobStatus(long jobId, JobStatusType status);

        Task<JobStatusType> GetJobStatus(long jobId);

        void CopyBlob(string source, string target);

        Task<bool> Exists(string fileName);

        Task DownloadBlobToStream(string fileName, Stream stream);

    }
}
