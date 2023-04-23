using NLog.Config;
using System.Runtime.CompilerServices;

namespace NLog.Targets.Bugsnag.Unit.Tests
{
    public abstract class BaseTest
    {
        protected BugsnagTarget GetBugsnagTarget()
        {
            return new BugsnagTarget
            {
                Name = "MyName",
                ApiKey = "abc1234",
                ReleaseStage = "testing",
                Endpoint = TestCasesSetup.BugsnagTestServer.Endpoint,
            };
        }

        protected Logger SetupNLog(BugsnagTarget target, [CallerMemberName] string loggerName = null)
        {
            var loggingConfiguration = new LoggingConfiguration();

            loggingConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, target));
            loggingConfiguration.AddTarget("Bugsnag", target);

            LogManager
                .Setup()
                .LoadConfiguration(loggingConfiguration);

            LogManager.ReconfigExistingLoggers();

            return LogManager.GetLogger(loggerName);
        }
    }
}
