using NLog.Targets.Bugsnag.Parameters;
using NUnit.Framework;
using System.Threading.Tasks;
using VerifyNUnit;

namespace NLog.Targets.Bugsnag.Unit.Tests
{
    public class AppLayoutTest : BaseTest
    {
        [Test]
        public async Task AppType_IsDefined_Then_ItIsUsed()
        {
            // Arrange
            var target = GetBugsnagTarget();
            target.App.AppType = "my appType with layout: ${logger}"; // fixed text + a layout renderer

            var logger = SetupNLog(target);

            // Act
            logger.Error("My message");
            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }

        [Test]
        public async Task Version_IsDefined_Then_ItIsUsed()
        {
            // Arrange
            var target = GetBugsnagTarget();
            target.App.Version = "${assembly-version:name=NLog:type=File}";

            var logger = SetupNLog(target);

            // Act
            logger.Error("My message");
            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }

        [Test]
        public async Task Assemblies_AreDefined_Then_TheyAreAdded()
        {
            // Arrange
            var target = GetBugsnagTarget();
            target.App.Assemblies.Add(new AppAssemblyParameter("Bugsnag"));
            target.App.Assemblies.Add(new AppAssemblyParameter("NLog"));

            var logger = SetupNLog(target);

            // Act
            logger.Error("My message");
            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }
    }
}