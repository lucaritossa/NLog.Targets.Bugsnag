using Bugsnag;
using Bugsnag.Payload;
using Newtonsoft.Json;
using NLog.Common;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets.Bugsnag.Parameters;
using System;
using System.Collections.Generic;
using Exception = System.Exception;

namespace NLog.Targets.Bugsnag
{
    [Target("Bugsnag")]
    public class BugsnagTarget : TargetWithContext
    {
        #region Private Fields

        private readonly Lazy<Client> _bugsnag;
        private const string _parameterNotConfigured = "{0} on target {1} is not configured";

        #endregion

        #region Public Properties

        [RequiredParameter]
        public Layout ApiKey { get; set; }

        [RequiredParameter]
        public Layout ReleaseStage { get; set; }

        public Layout Context { get; set; }
        public Layout ErrorClass { get; set; }

        public AppParameters App { get; } = new AppParameters();
        public RequestParameters Request { get; } = new RequestParameters();

        public Layout Endpoint { get; set; }

        #endregion

        #region Constructors

        public BugsnagTarget()
        {
            _bugsnag = new Lazy<Client>(() =>
            {
                var apiKey = RenderLogEvent(ApiKey, LogEventInfo.CreateNullEvent());
                if (string.IsNullOrEmpty(apiKey))
                    throw new NLogConfigurationException(string.Format(_parameterNotConfigured, nameof(ApiKey), nameof(BugsnagTarget)));

                var releaseStage = RenderLogEvent(ReleaseStage, LogEventInfo.CreateNullEvent());
                if (string.IsNullOrEmpty(releaseStage))
                    throw new NLogConfigurationException(string.Format(_parameterNotConfigured, nameof(ReleaseStage), nameof(BugsnagTarget)));

                var config = new Configuration
                {
                    ApiKey = apiKey,
                    ReleaseStage = releaseStage,
                };

                var endpoint = RenderLogEvent(Endpoint, LogEventInfo.CreateNullEvent());
                if (!string.IsNullOrWhiteSpace(endpoint))
                {
                    InternalLogger.Warn("{0}: Bugsnag endpoint was specified by configuration. Usually for testing purpose, the messages could not be sent to official Bugsnag service.", this);
                    config.Endpoint = new Uri(endpoint);
                }

                InternalLogger.Debug("{0}: Bugsnag client configured successfully", this);

                return new Client(config);
            });
        }

        #endregion

        #region Protected Methods

        protected override void Write(LogEventInfo logEvent)
        {
            if (logEvent.Exception != null)
            {
                InternalLogger.Debug("{0}: log event includes an exception", this);

                if (!string.IsNullOrEmpty(logEvent.FormattedMessage))
                    _bugsnag.Value.Breadcrumbs.Leave(logEvent.FormattedMessage, BreadcrumbType.Log, null);

                _bugsnag.Value.Notify(logEvent.Exception, logEvent.Level.ToSeverity(), report =>
                {
                    report.Event["unhandled"] = true;

                    AddAppInfo(logEvent, report);
                    AddRequest(logEvent, report);
                });

            }
            else if (!string.IsNullOrWhiteSpace(logEvent.Message))
            {
                InternalLogger.Debug("{0}: log event includes a simple message", this);

                var exception = new Exception(logEvent.FormattedMessage ?? logEvent.Message);

                _bugsnag.Value.Notify(exception, logEvent.Level.ToSeverity(), report =>
                {
                    var context = Context?.Render(logEvent) ?? logEvent.LoggerName;

                    var errorClass = GetErrorClass(logEvent);

                    report.Event.Context = context;

                    if (logEvent.HasProperties)
                        report.Event.Metadata.Add("properties", logEvent.Properties);

                    AddAppInfo(logEvent, report);
                    AddRequest(logEvent, report);

                    foreach (var ex in report.Event.Exceptions)
                    {
                        if (ex.ContainsKey("stacktrace"))
                        {
                            ex.Remove("stacktrace");

                            var stackTrace = new List<StackTraceLine>
                            {
                                new StackTraceLine(logEvent.CallerFilePath, logEvent.CallerLineNumber, logEvent.CallerMemberName, false)
                            };

                            ex.Add("stacktrace", stackTrace.ToArray());
                        }

                        ex.Remove("errorClass");
                        ex.Add("errorClass", errorClass);
                    }
                });
            }
        }

        #endregion

        #region Private Methods

        private void AddAppInfo(LogEventInfo logEvent, Report report)
        {
            var appVersion = App.Version != null ?
                App.Version.Render(logEvent) :
                Layout.FromString("${assembly-version:type=File}").Render(logEvent);

            report.Event.App.Add("version", appVersion);

            if (App.AppType != null)
                report.Event.App.Add("type", App.AppType.Render(logEvent));

            var appAssemblies = new List<string>();

            foreach (var appAssembly in App.Assemblies)
            {
                var version = Layout.FromString($"${{assembly-version:name={appAssembly.Name}:type=File}}").Render(logEvent);
                appAssemblies.Add($"{appAssembly.Name} {version}");
            }

            if (appAssemblies.Count > 0)
                report.Event.App.Add("assemblies", appAssemblies);
        }

        private void AddRequest(LogEventInfo logEvent, Report report)
        {
            if (!Request.Include)
                return;

            var request = new Request();

            if (Request.ClientIp.Include)
            {
                if (Request.ClientIp.Layout == null)
                    Request.ClientIp.Layout = "${aspnet-Request-Ip:CheckForwardedForHeader=true}";

                request.ClientIp = Request.ClientIp.Layout.Render(logEvent);
            }

            if (Request.Headers.Include)
            {
                if (Request.Headers.Layout == null)
                    Request.Headers.Layout = "${aspnet-request-headers:OutputFormat=JsonDictionary}";


                var headers = Request.Headers.Layout.Render(logEvent);
                Dictionary<string, string> headersDictionary;

                try
                {
                    headersDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(headers);
                }
                catch (Exception ex)
                {
                    InternalLogger.Log(ex, LogLevel.Error, "{0}: Unexpected error occurred during request headers deserialization. Headers will be added with single key 'key1' and value the original string", this);
                    headersDictionary = new Dictionary<string, string>
                    {
                        {"key1", headers}
                    };
                }

                request.Headers = headersDictionary;

            }

            if (Request.HttpMethod.Include)
            {
                if (Request.HttpMethod.Layout == null)
                    Request.HttpMethod.Layout = "${aspnet-request-method}";

                request.HttpMethod = Request.HttpMethod.Layout.Render(logEvent);
            }

            if (Request.Url.Include)
            {
                if (Request.Url.Layout == null)
                    Request.Url.Layout = "${aspnet-request-url:IncludePort=true:IncludeQueryString=true}";
                request.Url = Request.Url.Layout.Render(logEvent);
            }

            if (Request.Referrer.Include)
            {
                if (Request.Referrer.Layout == null)
                    Request.Referrer.Layout = "${aspnet-request-referrer}";

                request.Referer = Request.Referrer.Layout.Render(logEvent);
            }

            report.Event.Request = request;
        }

        private string GetErrorClass(LogEventInfo logEvent)
        {
            var result = ErrorClass?.Render(logEvent);
            if (result == null)
                result = $"{logEvent.CallerMemberName}:{logEvent.CallerLineNumber}";

            if (result == ":0")
                result = "NLog";

            return result;
        }

        #endregion
    }
}
