using NLog;
using NLog.Config;
using NLog.Targets;

namespace TNCovidBedApi
{
    internal class ApiLogger
    {
        private static ApiLogger loggerInstance = null;

        private readonly LoggingConfiguration logConfig = new LoggingConfiguration();

        private static string oldPath = "";

        private ApiLogger(string path = null)
        {
            string DirectoryPath = path == null ? System.IO.Directory.GetCurrentDirectory() : path;
            var fileTarget = new FileTarget("api_logger")
            {
                FileName = $"{DirectoryPath}/api-log.txt",
                Layout = "${longdate} ${level} ${message}  ${exception} ${event-properties:myProperty}"
            };
            fileTarget.DeleteOldFileOnStartup = false;
            logConfig.AddTarget(fileTarget);
            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
            NLog.LogManager.Configuration = logConfig;
        }

        public static ILogger GetLogger()
        {
            if (loggerInstance is null)
                throw new System.InvalidOperationException("");
            return loggerInstance.logConfig.LogFactory.GetLogger("api_logger");
        }

        public static ILogger GetLogger(string path)
        {
            if(oldPath == path)
                loggerInstance = loggerInstance ?? new ApiLogger(path);
            else
                loggerInstance = new ApiLogger(path);   
            return loggerInstance.logConfig.LogFactory.GetLogger("api_logger");
        }
    }
}