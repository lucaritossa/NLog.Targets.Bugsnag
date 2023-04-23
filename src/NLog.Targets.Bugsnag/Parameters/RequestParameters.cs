using NLog.Config;

namespace NLog.Targets.Bugsnag.Parameters
{
    [NLogConfigurationItem]
    public class RequestParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestParameters" /> class.
        /// </summary>
        public RequestParameters()
        { }

        public bool Include { get; set; }

        public RequestParameter ClientIp { get; set; } = new RequestParameter();
        public RequestParameter Headers { get; set; } = new RequestParameter();
        public RequestParameter HttpMethod { get; set; } = new RequestParameter();
        public RequestParameter Url { get; set; } = new RequestParameter();
        public RequestParameter Referrer { get; set; } = new RequestParameter();
    }
}