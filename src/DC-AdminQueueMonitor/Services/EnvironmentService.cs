using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSubmit.Services
{
    public class EnvironmentService
    {
        Dictionary<string, Environment> _environments = new Dictionary<string, Environment>();
        ESFA.DC.Serialization.Json.JsonSerializationService _serializationService;

        public EnvironmentService(string environmentsJson, ESFA.DC.Serialization.Json.JsonSerializationService ss)
        {
            _serializationService = ss;
            GetEnvironmentSettings(environmentsJson);

        }
        private void GetEnvironmentSettings(string environmentsJson)
        {
            if (string.IsNullOrEmpty(environmentsJson))
            {
                List<Environment> envs = new List<Environment>();
                var ci = new Environment()
                {
                    Name = "CI-Old",
                    JobApiUrl = "https://dc-ci-job-webapi-weu.ci.ase.dct.fasst.org.uk",
                    StorageAccount = "dcciilrstorageaccountwe",
                    StorageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=dcciilrstorageaccountwe;AccountKey=BcpAIywT/5W8VasdeSdxvCdUsH3VZ7APAYp84FlmenfhkzxCUBLynD8EqqZi8p7RE6ctCNAnJ+kKpI3GUNiIig==;EndpointSuffix=core.windows.net"
                };
                _environments.Add(ci.Name, ci);
                envs.Add(ci);
                environmentsJson = _serializationService.Serialize<List<Environment>>(envs);
            }
            else
            {
                var envs = _serializationService.Deserialize<List<Environment>>(environmentsJson);
                envs.ForEach(s => _environments.Add(s.Name, s));
            }
        }

        internal IEnumerable<string> EnvironmentNames()
        {
            return _environments.Keys;
        }

        internal Environment Environment(string name)
        {
            return _environments[name];
        }
    }
}
