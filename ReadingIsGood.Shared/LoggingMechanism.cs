using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Elasticsearch;
using ILogger = Serilog.ILogger;

namespace ReadingIsGood.Shared
{
    public static class LoggingMechanism
    {
        private static ILogger _logger;

        private static void InitializeLogger(string contextName, bool enableFileLog = false,
            bool enableConsoleLog = true, bool enableElasticLog = false)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithClientAgent()
                .Enrich.WithClientIp();

            var logLevel = ParseLevel(contextName);

            if (enableFileLog)
            {
                var defaultFilePath = ApplicationConfiguration.Instance.GetValue<string>("Logging:DefaultOutPutPath");
                loggerConfiguration.WriteTo.File(new CompactJsonFormatter(),
                    Path.Combine(defaultFilePath, contextName, $"{contextName}_log_.txt"),
                    logLevel,
                    rollingInterval: RollingInterval.Hour, fileSizeLimitBytes: 20000000L, rollOnFileSizeLimit: true,
                    retainedFileCountLimit: 720
                );
            }

            if (enableConsoleLog)
            {
                loggerConfiguration.WriteTo.Console(logLevel);
            }

            if (enableElasticLog)
            {
                loggerConfiguration.WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(
                        new Uri(ApplicationConfiguration.Instance.GetValue<string>("Logging:ElasticUrl")))
                    {
                        IndexFormat = $"{contextName.Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM-dd}",
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                        MinimumLogEventLevel = logLevel
                    });
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            _logger = Log.Logger;

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
        }

        private static LogEventLevel ParseLevel(string contextName)
        {
            LogLevel specifiedLogLevel;

            try
            {
                // checking whether a minimum log level has been specified on the context level
                specifiedLogLevel =
                    ApplicationConfiguration.Instance.GetValue<LogLevel>(
                        $"{contextName}:Logging:MinimumLevel");
            }
            catch
            {
                //logging is not set in the configuration file for the context section, returning minimum viable level from global section
                specifiedLogLevel = ApplicationConfiguration.Instance.GetValue<LogLevel>("Logging:LogLevel:Default");
            }

            switch (specifiedLogLevel)
            {
                case LogLevel.Trace:
                    return LogEventLevel.Verbose;
                case LogLevel.Debug:
                    return LogEventLevel.Debug;
                case LogLevel.Information:
                    return LogEventLevel.Information;
                case LogLevel.Warning:
                    return LogEventLevel.Warning;
                case LogLevel.Error:
                    return LogEventLevel.Error;
                case LogLevel.Critical:
                    return LogEventLevel.Fatal;
                case LogLevel.None:
                    return LogEventLevel.Error;
                default:
                    return LogEventLevel.Warning;
            }
        }

        public static Microsoft.Extensions.Logging.ILogger CreateLogger(string contextName, bool enableFileLog = false,
            bool enableConsoleLog = true, bool enableElasticLog = false)
        {
            InitializeLogger(contextName, enableFileLog, enableConsoleLog, enableElasticLog);
            return new SerilogLoggerProvider(Log.Logger).CreateLogger(contextName);
        }

        public static Serilog.ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    throw new ArgumentNullException("Firstly You have to call CreateLogger");
                }

                return _logger;
            }
        }
    }
}