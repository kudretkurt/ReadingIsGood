using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using ReadingIsGood.Shared;
using ReadingIsGood.Shared.CommonTypes;

namespace ReadingIsGood.OrderService
{
    internal class Program
    {
        private static IEndpointInstance _orderServiceCallbacksEndpoint;
        private static IEndpointInstance _orderServiceEndpoint;
        public static ILogger Logger;
        public static IMongoDbSettings MongoDbSettings;

        private static void Main(string[] args)
        {
            //Mongo Database Password:a6KrZ3DOmUBvwSTz
            //https://cloud.mongodb.com/v2/5fdf9b8d4780eb4f6fe21924#security/database/users
            Logger = LoggingMechanism.CreateLogger("orderService");
            MongoDbSettings = new MongoDbSettings
            {
                ConnectionString =
                    ApplicationConfiguration.Instance.GetValue<string>("OrderService:MongoDbConnectionString"),
                DatabaseName = ApplicationConfiguration.Instance.GetValue<string>("OrderService:MongoDbDatabaseName")
            };
            InitializeEndpoint().GetAwaiter().GetResult();
        }

        private static async Task InitializeEndpoint()
        {
            try
            {
                _orderServiceEndpoint = await Endpoint
                    .Start(EndpointConfigurations.GetOrderServiceEndpointConfiguration())
                    .ConfigureAwait(false);
                _orderServiceCallbacksEndpoint = await Endpoint
                    .Start(EndpointConfigurations.GetCallbackReceiverEndpointConfiguration())
                    .ConfigureAwait(false);

                while (true) Console.Read();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "OrderService catch error");
            }
            finally
            {
                await _orderServiceEndpoint?.Stop();
                await _orderServiceCallbacksEndpoint?.Stop();
            }
        }
    }
}