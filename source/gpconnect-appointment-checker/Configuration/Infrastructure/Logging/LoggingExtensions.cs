using gpconnect_appointment_checker.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using System.Data;

namespace gpconnect_appointment_checker.Configuration.Infrastructure
{
    public static class LoggingExtensions
    {
        public static IServiceCollection ConfigureLoggingServices(this IServiceCollection services, IConfiguration configuration)
        {
            var nLogConfiguration = new NLog.Config.LoggingConfiguration();

            var consoleTarget = AddConsoleTarget();
            var databaseTarget = AddDatabaseTarget(configuration);

            nLogConfiguration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, consoleTarget);
            nLogConfiguration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, databaseTarget);

            nLogConfiguration.AddTarget(consoleTarget);
            nLogConfiguration.AddTarget(databaseTarget);

            nLogConfiguration.Variables.Add("applicationVersion", ApplicationHelper.ApplicationVersion.GetAssemblyVersion());
            nLogConfiguration.Variables.Add("userId", null);
            nLogConfiguration.Variables.Add("userSessionId", null);

            LogManager.Configuration = nLogConfiguration;

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConfiguration(configuration.GetSection("Logging"));
            });

            return services;
        }

        private static DatabaseTarget AddDatabaseTarget(IConfiguration configuration)
        {
            var databaseTarget = new DatabaseTarget
            {
                Name = "Database",
                ConnectionString = configuration.GetConnectionString("DefaultConnection"),
                CommandType = CommandType.StoredProcedure,
                CommandText = "logging.log_error",
                DBProvider = "Npgsql.NpgsqlConnection, Npgsql"
            };

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@_application",
                Layout = "${var:applicationVersion}",
                DbType = DbType.String.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@_logged",
                Layout = "${date}",
                DbType = DbType.DateTime.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@_level",
                Layout = "${level:uppercase=true}",
                DbType = DbType.String.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@_message",
                Layout = "${message}",
                DbType = DbType.String.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@_logger",
                Layout = "${logger}",
                DbType = DbType.String.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@_callsite",
                Layout = "${callsite:filename=true}",
                DbType = DbType.String.ToString()
            });

            var exceptionLayout = new JsonLayout();
            exceptionLayout.Attributes.Add(new JsonAttribute("type", "${exception:format=Type}"));
            exceptionLayout.Attributes.Add(new JsonAttribute("message", "${exception:format=Message}"));
            exceptionLayout.Attributes.Add(new JsonAttribute("stacktrace", "${exception:format=StackTrace}"));
            exceptionLayout.Attributes.Add(new JsonAttribute("innerException", new JsonLayout
            {
                Attributes =
                {
                    new JsonAttribute("type", "${exception:format=:innerFormat=Type:MaxInnerExceptionLevel=1:InnerExceptionSeparator=}"),
                    new JsonAttribute("message", "${exception:format=:innerFormat=Message:MaxInnerExceptionLevel=1:InnerExceptionSeparator=}"),
                    new JsonAttribute("stacktrace", "${exception:format=:innerFormat=StackTrace:MaxInnerExceptionLevel=1:InnerExceptionSeparator=}")
                },
                RenderEmptyObject = false
            }, false));

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@_exception",
                Layout = exceptionLayout,
                DbType = DbType.String.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@_user_id",
                Layout = "${var:userId}",
                DbType = DbType.Int32.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@_user_session_id",
                Layout = "${var:userSessionId}",
                DbType = DbType.Int32.ToString()
            });

            return databaseTarget;
        }

        private static ConsoleTarget AddConsoleTarget()
        {
            var consoleTarget = new ConsoleTarget
            {
                Name = "Console",
                Layout = "${var:applicationVersion}|${date}|${level:uppercase=true}|${message}|${logger}|${callsite:filename=true}|${exception:format=stackTrace}|${var:userId}|${var:userSessionId}"
            };
            return consoleTarget;
        }
    }
}
