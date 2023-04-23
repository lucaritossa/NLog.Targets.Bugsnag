using Argon;
using NUnit.Framework;
using System.Threading.Tasks;
using VerifyNUnit;

namespace NLog.Targets.Bugsnag.Unit.Tests
{
    public class BasicParametersTest : BaseTest
    {
        #region Rainy Tests

        [Test]
        public Task ApiKey_NotSet_Then_ThrowsException()
        {
            // Arrange
            var target = GetBugsnagTarget();
            target.ApiKey = null;

            // Act
            var ex = Assert.Throws<NLogConfigurationException>(delegate
            {
                SetupNLog(target);

            });

            // Assert
            Assert.That(ex, Is.TypeOf<NLogConfigurationException>());
            Assert.That(ex.Message, Contains.Substring(nameof(BugsnagTarget.ApiKey)));
            return Task.CompletedTask;
        }

        [Test]
        public Task ReleaseStage_NotSet_Then_ThrowsException()
        {
            // Arrange
            var target = GetBugsnagTarget();
            target.ReleaseStage = null;

            // Act
            var ex = Assert.Throws<NLogConfigurationException>(delegate
            {
                SetupNLog(target);

            });

            // Assert
            Assert.That(ex, Is.TypeOf<NLogConfigurationException>());
            Assert.That(ex.Message, Contains.Substring(nameof(BugsnagTarget.ReleaseStage)));
            return Task.CompletedTask;
        }

        #endregion

        #region Sunny Tests

        [Test]
        public async Task ApiKey_IsDefined_Then_ItIsUsed()
        {
            // Arrange
            var target = GetBugsnagTarget();

            var logger = SetupNLog(target);

            // Act
            logger.Error("My message");
            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            Assert.IsTrue(actual.ContainsKey("apiKey"));
            Assert.AreEqual(target.ApiKey, actual.GetValue("apiKey").Value<string>());
        }

        [Test]
        public async Task ReleaseStage_IsDefined_Then_ItIsUsed()
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

        #endregion
    }
}