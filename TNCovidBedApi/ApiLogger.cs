using NLog;
using NLog.Config;
using NLog.Targets;

namespace TNCovidBedApi
{
    internal class ApiLogger
    {
        private static ApiLogger loggerInstance = null;

        private readonly LoggingConfiguration logConfig = new LoggingConfiguration();

        private ApiLogger()
        {
            var currentDir = System.IO.Directory.GetCurrentDirectory();
            var fileTarget = new FileTarget("target2")
            {
                FileName = $"{currentDir}/api-log.txt",
                Layout = "${longdate} ${level} ${message}  ${exception} ${event-properties:myProperty}"
            };
            fileTarget.DeleteOldFileOnStartup = false;
            logConfig.AddTarget(fileTarget);
            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
            NLog.LogManager.Configuration = logConfig;
        }

        public static ILogger GetLogger()
        {
            loggerInstance = loggerInstance ?? new ApiLogger();
            return loggerInstance.logConfig.LogFactory.GetLogger("api_logger");
        }
    }
}