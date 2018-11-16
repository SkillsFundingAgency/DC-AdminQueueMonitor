using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Dto;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Serialization.Interfaces;
using JobSubmit;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IJobQueueService _jobQueueService;
        private readonly CloudStorageSettings _cloudStorageSettings;
        private readonly IBespokeHttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly IJsonSerializationService _serializationService;

        public SubmissionService(
            IJobQueueService jobQueueService,
            CloudStorageSettings cloudStorageSettings,
            IBespokeHttpClient httpClient,
            ApiSettings apiSettings,
            IJsonSerializationService serializationService)
        {
            _jobQueueService = jobQueueService;
            _cloudStorageSettings = cloudStorageSettings;
            _httpClient = httpClient;
            _baseUrl = apiSettings?.JobAPIBaseUrl;
            _serializationService = serializationService;
        }

        public async Task<CloudBlobStream> GetBlobStream(string fileName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_cloudStorageSettings.ContainerName);
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            return await cloudBlockBlob.OpenWriteAsync();
        }

        public async Task<bool> Exists(string fileName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_cloudStorageSettings.ContainerName);
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            return await cloudBlockBlob.ExistsAsync();
        }

        public async Task DownloadBlobToStream(string fileName, Stream stream)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_cloudStorageSettings.ContainerName);
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            await cloudBlockBlob.DownloadToStreamAsync(stream);
        }


        public async void CopyBlob(string source, string target)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_cloudStorageSettings.ContainerName);
            CloudBlockBlob sourceBlob = cloudBlobContainer.GetBlockBlobReference(source);
            CloudBlockBlob targetBlob = cloudBlobContainer.GetBlockBlobReference(target);
            var result = await targetBlob.StartCopyAsync(sourceBlob);
            targetBlob.FetchAttributes();
            while ( targetBlob.CopyState.Status == CopyStatus.Pending )
            {
                Thread.Sleep(500);
                targetBlob.FetchAttributes();
            }
        }

        public async Task<long> SubmitIlrJob(
            ESFA.DC.Jobs.Model.Enums.JobType jobType,
            string fileName,
            decimal fileSizeBytes,
            string submittedBy,
            long ukprn,
            string collectionName,
            int period,
            bool isFirstStageOnly,
            string emailAddress
            )
        {
            var job = new FileUploadJob()
            {
                Ukprn = ukprn,
                DateTimeSubmittedUtc = DateTime.UtcNow,
                Priority = 1,
                Status = JobStatusType.Ready,
                SubmittedBy = submittedBy,
                FileName = fileName,
                IsFirstStage = isFirstStageOnly,
                StorageReference = _cloudStorageSettings.ContainerName,
                FileSize = fileSizeBytes,
                CollectionName = collectionName,
                PeriodNumber = period,
                NotifyEmail = emailAddress,
                JobType = jobType
            };
            return await _jobQueueService.AddJobAsync(job);
        }

        public async Task<FileUploadJob> GetJob(long ukprn, long jobId)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/job/{ukprn}/{jobId}");
            return _serializationService.Deserialize<FileUploadJob>(data);
        }

        public async Task<JobStatusType> GetJobStatus(long jobId)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/job/{jobId}/status");
            return _serializationService.Deserialize<JobStatusType>(data);
        }

        public async Task<IEnumerable<FileUploadJob>> GetAllJobs(long ukprn)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/job/{ukprn}");
            return _serializationService.Deserialize<IEnumerable<FileUploadJob>>(data);
        }

        public async Task<IEnumerable<FileUploadJob>> GetAllJobs()
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/job");
            return _serializationService.Deserialize<IEnumerable<FileUploadJob>>(data);
        }

        public async Task<string> UpdateJobStatus(long jobId, JobStatusType status)
        {
            var job = new JobStatusDto()
            {
                JobId = jobId,
                JobStatus = (int)status,
                NumberOfLearners = 0
                
            };
            return await _httpClient.SendDataAsync($"{_baseUrl}/job/status", job);
        }

        internal async Task<long> GetBlobFilesize(string fp)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_cloudStorageSettings.ContainerName);
            var blob = cloudBlobContainer.GetBlockBlobReference(fp);
            if (blob.Exists())
            {
                blob.FetchAttributes();
                return blob.Properties.Length;
            }
            return 0;
        }

        public IEnumerable<ILRPhysicalFile> ExistingStorageFilenames()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_cloudStorageSettings.ContainerName);
            List<ILRPhysicalFile> files = new List<ILRPhysicalFile>(100);
            var blobs = cloudBlobContainer.ListBlobs( useFlatBlobListing:true);
            foreach( var blob in blobs )
            {
                bool directory = false;
                if (blob is CloudBlobDirectory)
                {
                    directory = true;
                    // https://sfa-gov-uk.visualstudio.com/DCT/_git/DC-Job-Manager
                }
                var count = blob.Uri.Segments.Count();
                string filename = blob.Uri.Segments[count-2] + blob.Uri.Segments[count - 1];
                if (blob.Uri.Segments.Last().StartsWith("ILR") && blob.Uri.Segments[count - 2].Length == 9)
                {
                    files.Add(new ILRPhysicalFile()
                    {
                        Filename = filename,
                        Size = 0,
                        Directory = directory
                    });
                }
            }
            return files;
        }
    }
}
