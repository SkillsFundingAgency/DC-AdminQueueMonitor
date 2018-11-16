using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Interface;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DC.Web.Ui.Services.Interfaces
{
    public interface ICollectionsManagementService
    {
        /// <summary>
        /// Which collection types can this organisation return data for, e.g. ILR, EAS
        /// </summary>
        /// <param name="ukprn"></param>
        /// <returns></returns>
        Task<IEnumerable<CollectionType>> GetOrgCollectionTypes(long ukprn);
        /// <summary>
        /// Turns collection types into actual collections; e.g. the difference between ILR and ILR1819 
        /// </summary>
        /// <param name="ukprn"></param>
        /// <param name="collectionTypes"></param>
        /// <returns></returns>
        Task<IEnumerable<Collection>> GetOrgCollections(long ukprn, IEnumerable<CollectionType> collectionTypes);
    }
}
