using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.Jobs.Model;

namespace DC.Web.Ui.Services.Services
{
    public class JobQueueService : IJobQueueService
    {
        private readonly string _apiBaseUrl;
        private readonly IBespokeHttpClient _httpClient;

        public JobQueueService(ApiSettings apiSettings, IBespokeHttpClient httpClient)
        {
            _apiBaseUrl = apiSettings.JobAPIBaseUrl;
            _httpClient = httpClient;
        }

        public async Task<long> AddJobAsync(FileUploadJob job)
        {
            var response = await _httpClient.SendDataAsync($"{_apiBaseUrl}/job", job);
            long.TryParse(response, out var result);
            return result;
        }
    }
}
