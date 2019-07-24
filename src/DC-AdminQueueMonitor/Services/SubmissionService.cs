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

        //public async Task<CloudBlobStream> GetBlobStream(string fileName)
        //{
        //    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
        //    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        //    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_cloudStorageSettings.ContainerName);
        //    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

        //    return await cloudBlockBlob.OpenWriteAsync();
        //}

        //public async Task<bool> Exists(string fileName)
        //{
        //    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
        //    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        //    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_cloudStorageSettings.ContainerName);
        //    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

        //    return await cloudBlockBlob.ExistsAsync();
        //}

        //public async Task DownloadBlobToStream(string fileName, Stream stream)
        //{
        //    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
        //    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        //    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_cloudStorageSettings.ContainerName);
        //    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

        //    await cloudBlockBlob.DownloadToStreamAsync(stream);
        //}


        //public async void CopyBlob(string source, string target)
        //{
        //    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
        //    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        //    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_cloudStorageSettings.ContainerName);
        //    CloudBlockBlob sourceBlob = cloudBlobContainer.GetBlockBlobReference(source);
        //    CloudBlockBlob targetBlob = cloudBlobContainer.GetBlockBlobReference(target);
        //    var result = await targetBlob.StartCopyAsync(sourceBlob);
        //    targetBlob.FetchAttributes();
        //    while ( targetBlob.CopyState.Status == CopyStatus.Pending )
        //    {
        //        Thread.Sleep(500);
        //        targetBlob.FetchAttributes();
        //    }
        //}

        //public async Task<FileUploadJob> GetJob(long ukprn, long jobId)
        //{
        //    var data = await _httpClient.GetDataAsync($"{_baseUrl}/job/{ukprn}/{jobId}");
        //    return _serializationService.Deserialize<FileUploadJob>(data);
        //}

        //public async Task<JobStatusType> GetJobStatus(long jobId)
        //{
        //    var data = await _httpClient.GetDataAsync($"{_baseUrl}/job/{jobId}/status");
        //    return _serializationService.Deserialize<JobStatusType>(data);
        //}

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

        //internal async Task<long> GetBlobFilesize(string fp)
        //{
        //    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
        //    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        //    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_cloudStorageSettings.ContainerName);
        //    var blob = cloudBlobContainer.GetBlockBlobReference(fp);
        //    if (blob.Exists())
        //    {
        //        blob.FetchAttributes();
        //        return blob.Properties.Length;
        //    }
        //    return 0;
        //}
    }
}
