using NLog.Config;
using NLog.Layouts;
using System.Collections.Generic;

namespace NLog.Targets.Bugsnag.Parameters
{
    [NLogConfigurationItem]
    public class AppParameters
    {
        public Layout AppType { get; set; }
        public Layout Version { get; set; }

        [ArrayParameter(typeof(AppAssemblyParameter), "assembly")]
        public IList<AppAssemblyParameter> Assemblies { get; } = new List<AppAssemblyParameter>();

        #region Constructors

        public AppParameters()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppParameters" /> class.
        /// </summary>
        public AppParameters(Layout type, Layout version, IList<AppAssemblyParameter> assemblies)
        {
            AppType = type;
            Version = version;
            Assemblies = assemblies;
        }

        #endregion
    }
}