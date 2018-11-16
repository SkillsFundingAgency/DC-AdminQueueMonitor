using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSubmit
{
    public class Environment
    {
        public string Name { get; set; }
        public string JobApiUrl { get; set; }
        public string StorageAccount { get; set; }
        public string StorageAccountConnectionString { get; set; }
        public string AppLogsConnectionString { get; set; }
        public string AuditConnectionString { get; set; }
    }
}
