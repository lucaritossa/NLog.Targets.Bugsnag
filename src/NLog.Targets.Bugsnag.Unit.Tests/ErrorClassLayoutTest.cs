using NUnit.Framework;
using System.Threading.Tasks;
using VerifyNUnit;

namespace NLog.Targets.Bugsnag.Unit.Tests
{
    public class ErrorClassLayoutTest : BaseTest
    {
        [Test]
        public async Task IsDefined_Then_ItIsUsed()
        {
            // Arrange
            var target = GetBugsnagTarget();
            target.ErrorClass= "my custom errorClass with layout: ${logger}"; // fixed text + a layout renderer

            var logger = SetupNLog(target);

            // Act
            logger.Error("My message");
            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }

        [Test]
        public async Task IsNotDefined_Then_Default_IsUsed()
        {
            // Arrange
            var target = GetBugsnagTarget();

            var logger = SetupNLog(target);

            // Act
            logger.Error("My message");
            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }

    }
}