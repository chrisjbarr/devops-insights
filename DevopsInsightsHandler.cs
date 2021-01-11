using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DevOps.Insights
{
    public class DevopsInsightsHandler : IHttpHandler
    {
        private static IEnumerable<string> applicationNamespacePrefixes = null;

        public DevopsInsightsHandler()
        {
            DevopsInsightsHandler.applicationNamespacePrefixes = (ConfigurationManager.AppSettings["devops.insights:ApplicationNamespacePrefixes"] ?? string.Empty).Split(';');
        }

        #region IHttpHandler Implementation
        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.
        /// </summary>
        /// 
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.
        /// </summary>
        /// 
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        /// 
        public void ProcessRequest(HttpContext context)
        {
            InsightsResult result = new InsightsResult
            {
                LocalAssemblyInformation = AssemblyVersions.RetrieveLocalAssemblies(),
                ThirdPartyAssemblyInformation = AssemblyVersions.RetrieveThirdPartyAssemblies()
            };

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["devops.insights:ApplicationNamespacePrefixes"]))
            {
                result.ApplicationAssemblyInformation = AssemblyVersions.RetrieveFilteredAssemblies(DevopsInsightsHandler.applicationNamespacePrefixes);
            }

            if (ConfigurationManager.ConnectionStrings != null && ConfigurationManager.ConnectionStrings.Count > 0)
            {
                result.DatabaseConnectionInformation = DatabaseConnections.RetrieveDatabaseConnectionInformation(ConfigurationManager.ConnectionStrings);
            }

            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(result));
        }
        #endregion
    }
}
