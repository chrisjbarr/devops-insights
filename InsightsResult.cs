using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOps.Insights
{
    [Serializable()]
    public class InsightsResult
    {
        public IEnumerable<AssemblyInformation> LocalAssemblyInformation { get; set; }
        public IEnumerable<AssemblyInformation> ApplicationAssemblyInformation { get; set; }
        public IEnumerable<AssemblyInformation> ThirdPartyAssemblyInformation { get; set; }
        public IEnumerable<DatabaseConnectionInformation> DatabaseConnectionInformation { get; set; }
    }
}
