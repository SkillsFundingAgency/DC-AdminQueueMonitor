using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface IJobQueueService
    {
        Task<long> AddJobAsync(FileUploadJob job);
    }
}
