using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using NServiceBus.Features;
using ReadingIsGood.ProductService.Context;
using ReadingIsGood.ProductService.Policies;
using ReadingIsGood.ProductService.Repositories;
using ReadingIsGood.Shared;

namespace ReadingIsGood.ProductService
{
    public static class EndpointConfigurations
    {
        public static EndpointConfiguration GetProductServiceEndpointConfiguration()
        {
            var endpointName =
                ApplicationConfiguration.Instance.GetValue<string>("ProductService:EndpointName");

            var errorQueueName =
                ApplicationConfiguration.Instance.GetValue<string>("ProductService:ErrorQueueName");

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
            recoverability.CustomPolicy(new OptimisticRetryPolicy().Retry);
            recoverability.Immediate(immediate => { immediate.NumberOfRetries(0); });
            recoverability.Delayed(delayed =>
            {
                delayed.NumberOfRetries(3);
                delayed.TimeIncrease(TimeSpan.FromSeconds(20));
            });


            var productServiceDbContextOptionsBuilder = new DbContextOptionsBuilder<ProductServiceDbContext>();
            productServiceDbContextOptionsBuilder.UseSqlServer(
                ApplicationConfiguration.Instance.GetValue<string>("ProductService:DatabaseConnectionString"));

            endpointConfiguration.RegisterComponents(configureComponents =>
            {
                configureComponents.ConfigureComponent<IProductRepository>(
                    () => new ProductRepository(
                        new ProductServiceDbContext(productServiceDbContextOptionsBuilder.Options)),
                    DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent(() => Program.Logger, DependencyLifecycle.SingleInstance);
            });

            endpointConfiguration.DisableFeature<AutoSubscribe>();


            return endpointConfiguration;
        }

        public static EndpointConfiguration GetCallbackReceiverEndpointConfiguration()
        {
            var endpointName =
                ApplicationConfiguration.Instance.GetValue<string>("ProductService:CallbacksReceiverEndpointName");

            var errorQueueName =
                ApplicationConfiguration.Instance.GetValue<string>("ProductService:CallbacksReceiverErrorQueueName");

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
            recoverability.CustomPolicy(new OptimisticRetryPolicy().Retry);
            recoverability.Immediate(immediate => { immediate.NumberOfRetries(3); });
            recoverability.Delayed(delayed => { delayed.NumberOfRetries(0); });

            var productServiceDbContextOptionsBuilder = new DbContextOptionsBuilder<ProductServiceDbContext>();
            productServiceDbContextOptionsBuilder.UseSqlServer(
                ApplicationConfiguration.Instance.GetValue<string>("ProductService:DatabaseConnectionString"));

            endpointConfiguration.RegisterComponents(configureComponents =>
            {
                configureComponents.ConfigureComponent<IProductRepository>(
                    () => new ProductRepository(
                        new ProductServiceDbContext(productServiceDbContextOptionsBuilder.Options)),
                    DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent(() => Program.Logger, DependencyLifecycle.SingleInstance);
            });

            endpointConfiguration.DisableFeature<AutoSubscribe>();

            return endpointConfiguration;
        }
    }
}