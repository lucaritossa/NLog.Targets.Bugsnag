using NLog.Config;

namespace NLog.Targets.Bugsnag.Parameters
{
    [NLogConfigurationItem]
    public class AppAssemblyParameter
    {
        /// <summary>
        /// Gets or sets the database parameter name.
        /// </summary>
        [RequiredParameter]
        public string Name { get; set; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppAssemblyParameter" /> class.
        /// </summary>
        public AppAssemblyParameter()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppAssemblyParameter" /> class.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        public AppAssemblyParameter(string name)
        {
            Name = name;
        }

        #endregion
    }
}