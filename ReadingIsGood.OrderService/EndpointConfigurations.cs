using System;
using System.Data.SqlClient;
using NServiceBus;
using NServiceBus.Features;
using ReadingIsGood.OrderService.Entities;
using ReadingIsGood.OrderService.Repositories;
using ReadingIsGood.Shared;

namespace ReadingIsGood.OrderService
{
    public static class EndpointConfigurations
    {
        public static EndpointConfiguration GetCallbackReceiverEndpointConfiguration()
        {
            var endpointName =
                ApplicationConfiguration.Instance.GetValue<string>("OrderService:CallbacksReceiverEndpointName");

            var errorQueueName =
                ApplicationConfiguration.Instance.GetValue<string>("OrderService:CallbacksReceiverErrorQueueName");

            var transportConnectionString =
                ApplicationConfiguration.Instance.GetValue<string>("ServiceBus:TransportConnectionString");

            var endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.SendFailedMessagesTo(errorQueueName);
            endpointConfiguration.EnableCallbacks();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.PurgeOnStartup(true);
            endpointConfiguration.MakeInstanceUniquelyAddressable(Environment.MachineName);

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>().UseConventionalRoutingTopology();
            transport.ConnectionString(transportConnectionString);
            transport.DisableRemoteCertificateValidation();

            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            var recoverability = endpointConfiguration.Recoverability();
            recoverability.Immediate(immediate => { immediate.NumberOfRetries(0); });
            recoverability.Delayed(delayed => { delayed.NumberOfRetries(0); });

            endpointConfiguration.DisableFeature<AutoSubscribe>();

            endpointConfiguration.RegisterComponents(configureComponents =>
            {
                configureComponents.ConfigureComponent(() => Program.Logger, DependencyLifecycle.SingleInstance);

                configureComponents.ConfigureComponent(() => Program.MongoDbSettings,
                    DependencyLifecycle.SingleInstance);

                configureComponents.ConfigureComponent<IMongoRepository<Order>>(
                    () => new MongoRepository<Order>(Program.MongoDbSettings),
                    DependencyLifecycle.InstancePerUnitOfWork);
            });


            return endpointConfiguration;
        }

        public static EndpointConfiguration GetOrderServiceEndpointConfiguration()
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


            endpointConfiguration.RegisterComponents(configureComponents =>
            {
                configureComponents.ConfigureComponent(() => Program.Logger, DependencyLifecycle.SingleInstance);

                configureComponents.ConfigureComponent(() => Program.MongoDbSettings,
                    DependencyLifecycle.SingleInstance);

                configureComponents.ConfigureComponent<IMongoRepository<Order>>(
                    () => new MongoRepository<Order>(Program.MongoDbSettings),
                    DependencyLifecycle.InstancePerUnitOfWork);
            });


            return endpointConfiguration;
        }
    }
}