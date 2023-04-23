using NUnit.Framework;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyNUnit;

namespace NLog.Targets.Bugsnag.Unit.Tests
{
    public class StackTraceTest : BaseTest
    {
        public class MyException : Exception
        {
            public MyException()
            {
            }

            public MyException(string message) : base(message)
            {
            }

            public MyException(string message, Exception inner) : base(message, inner)
            {
            }
        }

        [Test]
        public async Task CallerInfo_IsDefined_Then_IsUsedInStackTrace()
        {
            // Arrange
            var target = GetBugsnagTarget();
            var logger = SetupNLog(target);

            // Act
            LogMessage(logger, "My message");

            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }

        [Test]
        public async Task Exception_IsLogged_Then_StackTraceIsPopulatedd()
        {
            // Arrange
            var target = GetBugsnagTarget();
            var logger = SetupNLog(target);

            // Act
            try
            {
                MyMethod_That_Throws_An_Exception();
            }
            catch (Exception ex)
            {
                LogException(logger, ex);
            }

            var actual = await TestCasesSetup.BugsnagTestServer.GetRequest();

            // Assert
            await Verifier.Verify(actual).ToTask();
        }

        private void LogMessage(Logger logger, string message,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = null)
        {
            var eventInfo = new LogEventInfo
            {
                Level = LogLevel.Error,
                Message = message
            };

            eventInfo.SetCallerInfo(nameof(StackTraceTest), memberName, filePath, lineNumber);

            logger.Log(eventInfo);
        }

        private void LogException(Logger logger, Exception ex,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = null)
        {
            var eventInfo = new LogEventInfo
            {
                Level = LogLevel.Error,
                Exception = ex
            };

            eventInfo.SetCallerInfo(nameof(StackTraceTest), memberName, filePath, lineNumber);

            logger.Log(eventInfo);
        }

        private void MyMethod_That_Throws_An_Exception()
        {
            var ex = new MyException("this is the message of my exception");
            throw ex;
        }
    }
}