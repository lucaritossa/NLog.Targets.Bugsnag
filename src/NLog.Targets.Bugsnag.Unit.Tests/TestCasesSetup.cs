using Argon;
using NUnit.Framework;
using System.IO;
using VerifyNUnit;
using VerifyTests;

namespace NLog.Targets.Bugsnag.Unit.Tests
{
    [SetUpFixture]
    public class TestCasesSetup
    {
        public static BugsnagTestServer BugsnagTestServer { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            VerifySetup();
            InitBugsnagTestServer();
            InitLogManager();
        }

        private void VerifySetup()
        {
            /*
             * -----------------------------------------
             * https://github.com/VerifyTests/Verify
             * -----------------------------------------
             */

            VerifierSettings.SortPropertiesAlphabetically();

            Verifier.DerivePathInfo((sourceFile, projectDirectory, type, method) =>
            {
                var currentDirectory = Path.GetDirectoryName(sourceFile);

                var pathInfo = new PathInfo(
                    directory: Path.Combine(currentDirectory, "expected"),
                    typeName: type.Name,
                    methodName: method.Name);

                return pathInfo;
            });

            VerifierSettings.AddExtraSettings(_ => _.DefaultValueHandling = DefaultValueHandling.Include);
            VerifierSettings.DontIgnoreEmptyCollections();
            VerifierSettings.ScrubMembers("device");
        }

        private void InitBugsnagTestServer()
        {
            BugsnagTestServer = new BugsnagTestServer();
            BugsnagTestServer.Start();
        }

        private static void InitLogManager()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "nlog-internal.txt");

            if (File.Exists(filePath))
                File.Delete(filePath);

            LogManager
                .Setup()
                .SetupInternalLogger(builder =>
                {
                    builder.LogToFile(filePath);
                    builder.SetMinimumLogLevel(LogLevel.Trace);
                    builder.LogFactory.ThrowExceptions = true;
                });
        }
    }
}
