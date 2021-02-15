using System;
using System.Data.SqlClient;
using Infrastructure.Shared;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using Serilog.Extensions.Logging;

namespace ReadingIsGood.OrderService
{
    public static class EndpointConfigurations
    {
        public static EndpointConfiguration GetCallbackReceiverEndpointConfiguration(Serilog.ILogger logger)
        {
            var endpointName =
                ApplicationConfiguration.Instance.GetValue<string>("OrderService:CallbacksReceiverEndpointName");

            var errorQueueName =
                ApplicationConfiguration.Instance.GetValue<string>("OrderService:CallbacksReceiverErrorQueueName");

            var transportConnectionString =
                ApplicationConfiguration.Instance.GetValue<string>("ServiceBus:TransportConnectionString");

            var endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.SendFailedMessagesTo(errorQueueName);
            endpointConfiguration.EnableCallbacks(makesRequests: true);
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.PurgeOnStartup(true);
            endpointConfiguration.MakeInstanceUniquelyAddressable(Environment.MachineName);

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>().UseConventionalRoutingTopology();
            transport.ConnectionString(transportConnectionString);
            transport.DisableRemoteCertificateValidation();

            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            
            var factory = LogManager.Use<SerilogFactory>();
            factory.WithLogger(logger);

            var recoverability = endpointConfiguration.Recoverability();
            recoverability.Immediate(immediate => { immediate.NumberOfRetries(0); });
            recoverability.Delayed(delayed => { delayed.NumberOfRetries(0); });

            endpointConfiguration.DisableFeature<AutoSubscribe>();


            return endpointConfiguration;
        }

        public static EndpointConfiguration GetOrderServiceEndpointConfiguration(Serilog.ILogger logger)
        {
            var endpointName =
                ApplicationConfiguration.Instance.GetValue<string>("OrderService:EndpointName");

            var errorQueueName =
                ApplicationConfiguration.Instance.GetValue<string>("OrderService:ErrorQueueName");

            var transportConnectionString =
                ApplicationConfiguration.Instance.GetValue<string>("ServiceBus:TransportConnectionString");

            var servicebusDatabaseConnectionString =
                ApplicationConfiguration.Instance.GetValue<string>("ServiceBus:DatabaseConnectionString");

            var endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.EnableDurableMessages();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.SendFailedMessagesTo(errorQueueName);

            var factory = LogManager.Use<SerilogFactory>();
            factory.WithLogger(logger);

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>().UseConventionalRoutingTopology();
            transport.ConnectionString(transportConnectionString);
            transport.DisableRemoteCertificateValidation();

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(() => new SqlConnection(servicebusDatabaseConnectionString));

            var recoverability = endpointConfiguration.Recoverability();
            recoverability.Immediate(immediate => { immediate.NumberOfRetries(0); });
            recoverability.Delayed(delayed =>
            {
                delayed.NumberOfRetries(3);
                delayed.TimeIncrease(TimeSpan.FromSeconds(20));
            });

            endpointConfiguration.DisableFeature<AutoSubscribe>();


            return endpointConfiguration;
        }
    }
}