using NLog.Config;
using NLog.Layouts;

namespace NLog.Targets.Bugsnag.Parameters
{
    [NLogConfigurationItem]
    public class RequestParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestParameter" /> class.
        /// </summary>
        public RequestParameter()
        { }

        public RequestParameter(bool include, Layout layout)
        {
            Include = include;
            Layout = layout;
        }

        public bool Include { get; set; } = true;

        public Layout Layout { get; set; }
    }
}