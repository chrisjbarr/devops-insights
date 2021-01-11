using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOps.Insights
{
    public class DatabaseConnectionInformation
    {
        public string ServerName { get; set; }
        public string ConnectionStringName { get; set; }
        public string Version { get; set; }
        public string StatusMessage { get; set; }
        public bool IsAlive { get; set; }
    }
}
