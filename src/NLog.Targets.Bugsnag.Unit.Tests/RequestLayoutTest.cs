using NLog.Targets.Bugsnag.Parameters;
using NUnit.Framework;
using System.Threading.Tasks;
using VerifyNUnit;

namespace NLog.Targets.Bugsnag.Unit.Tests
{
    public class RequestLayoutTest : BaseTest
    {
        [Test]
        public async Task Request_IsIncluded_But_NoProperties_Then_RequestWillBeEmpty()
        {
            // Arrange
            var target = GetDefaultTarget();

            //target.Request.ClientIp.Include = true;
            //target.Request.ClientIp.Layout = "my clientIp with layout: ${logger}"; // fixed text + a layout renderer

            var logger = SetupNLog(target);

            // Act
            logger.Error("My message");
            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }

        [Test]
        public async Task ClientIp_IsDefined_Then_ItIsUsed()
        {
            // Arrange
            var target = GetDefaultTarget();

            target.Request.ClientIp.Include = true;
            target.Request.ClientIp.Layout = "my clientIp with layout: ${logger}"; // fixed text + a layout renderer

            var logger = SetupNLog(target);

            // Act
            logger.Error("My message");
            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }

        [Test]
        public async Task Headers_IsDefinedAsJson_Then_ItIsUsed_And_Deserialized()
        {
            // Arrange
            var target = GetDefaultTarget();

            target.Request.Headers.Include = true;
            target.Request.Headers.Layout = "{\"key1\":\"value1\",\"key2\":\"value2\"}";

            var logger = SetupNLog(target);

            // Act
            logger.Error("My message");
            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }

        [Test]
        public async Task Headers_IsDefined_But_CannotBeDeserialized_Then_ItIsUsed_As_Is()
        {
            // Arrange
            var target = GetDefaultTarget();

            target.Request.Headers.Include = true;
            target.Request.Headers.Layout = "my headers with layout: ${logger}"; // fixed text + a layout renderer

            var logger = SetupNLog(target);

            // Act
            logger.Error("My message");
            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }

        [Test]
        public async Task HttpMethod_IsDefined_Then_ItIsUsed()
        {
            // Arrange
            var target = GetDefaultTarget();

            target.Request.HttpMethod.Include = true;
            target.Request.HttpMethod.Layout = "my httpMethod with layout: ${logger}"; // fixed text + a layout renderer

            var logger = SetupNLog(target);

            // Act
            logger.Error("My message");
            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }

        [Test]
        public async Task Referer_IsDefined_Then_ItIsUsed()
        {
            // Arrange
            var target = GetDefaultTarget();

            target.Request.Referrer.Include = true;
            target.Request.Referrer.Layout = "my referer with layout: ${logger}"; // fixed text + a layout renderer

            var logger = SetupNLog(target);

            // Act
            logger.Error("My message");
            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }

        [Test]
        public async Task Url_IsDefined_Then_ItIsUsed()
        {
            // Arrange
            var target = GetDefaultTarget();

            target.Request.Url.Include = true;
            target.Request.Url.Layout = "my url with layout: ${logger}"; // fixed text + a layout renderer

            var logger = SetupNLog(target);

            // Act
            logger.Error("My message");
            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }

        #region Private Methods

        private BugsnagTarget GetDefaultTarget()
        {
            var target = GetBugsnagTarget();

            target.Request.Include = true;

            target.Request.ClientIp.Include = false;
            target.Request.Headers.Include = false;
            target.Request.HttpMethod.Include = false;
            target.Request.Referrer.Include = false;
            target.Request.Url.Include = false;

            return target;
        }

        #endregion
    }
}