using System;
using System.Threading.Tasks;
using Infrastructure.Shared;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace ReadingIsGood.OrderService
{
    internal class Program
    {
        private static IEndpointInstance _orderServiceCallbacksEndpoint;
        private static IEndpointInstance _orderServiceEndpoint;
        private static ILogger _logger;

        private static void Main(string[] args)
        {
            //Mongo Database Password:a6KrZ3DOmUBvwSTz
            //https://cloud.mongodb.com/v2/5fdf9b8d4780eb4f6fe21924#security/database/users
            _logger = LoggingMechanism.CreateLogger("orderService");
            InitializeEndpoint().GetAwaiter().GetResult();
        }

        private static async Task InitializeEndpoint()
        {
            try
            {
                _orderServiceEndpoint = await Endpoint
                    .Start(EndpointConfigurations.GetOrderServiceEndpointConfiguration(LoggingMechanism.Logger))
                    .ConfigureAwait(false);
                _orderServiceCallbacksEndpoint = await Endpoint
                    .Start(EndpointConfigurations.GetCallbackReceiverEndpointConfiguration(LoggingMechanism.Logger))
                    .ConfigureAwait(false);

                while (true) Console.Read();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "OrderService catch error");
            }
            finally
            {
                await _orderServiceEndpoint?.Stop();
                await _orderServiceCallbacksEndpoint?.Stop();
            }
        }
    }
}