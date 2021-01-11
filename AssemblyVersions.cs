using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOps.Insights
{
    public class AssemblyVersions
    {
        #region consts
        private const string NAMESPACE_PREFIX_LONG = "Local";
        private const string NAMESPACE_PREFIX_SHORT = "L";
        #endregion

        #region private methods
        /// <summary>
        /// Retrieves the a list of assemblies
        /// </summary>
        /// <param name="nameStartsWithPredicate">Predicate in the form of assembly name starting with.</param>
        /// 
        private static IEnumerable<AssemblyInformation> RetrieveAssemblies(Func<string, bool> nameStartsWithPredicate)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                                          .Where(a => nameStartsWithPredicate == null || nameStartsWithPredicate(a.FullName))
                                          .Select(a => new AssemblyInformation()
                                          {
                                              Name = a.GetName().Name,
                                              Version = a.GetName().Version.ToString()
                                          });
        }
        #endregion

        #region public methods
        /// <summary>
        /// Retrieves a list of local assembly dependencies
        /// </summary>
        /// 
        public static IEnumerable<AssemblyInformation> RetrieveLocalAssemblies()
        {
            return AssemblyVersions.RetrieveAssemblies((a) => a.StartsWith(AssemblyVersions.NAMESPACE_PREFIX_LONG, StringComparison.InvariantCultureIgnoreCase) ||
                                                              a.StartsWith(AssemblyVersions.NAMESPACE_PREFIX_SHORT, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Retrieves a list of third party assembly dependencies
        /// </summary>
        /// 
        public static IEnumerable<AssemblyInformation> RetrieveThirdPartyAssemblies()
        {
            return AssemblyVersions.RetrieveAssemblies((a) => !a.StartsWith(AssemblyVersions.NAMESPACE_PREFIX_LONG, StringComparison.InvariantCultureIgnoreCase) &&
                                                              !a.StartsWith(AssemblyVersions.NAMESPACE_PREFIX_SHORT, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Retrieves a list of assembly dependencies given the filter
        /// </summary>
        /// <param name="nameStartsWith">The assembly name starts with.</param>
        /// 
        public static IEnumerable<AssemblyInformation> RetrieveFilteredAssemblies(IEnumerable<string> nameStartsWithList)
        {
            List<AssemblyInformation> filteredAssemblies = new List<AssemblyInformation>();

            foreach (string nameStartsWith in nameStartsWithList)
            {
                filteredAssemblies.AddRange(AssemblyVersions.RetrieveAssemblies((a) => a.StartsWith(nameStartsWith)));
            }

            return filteredAssemblies;
        }
        #endregion
    }
}
