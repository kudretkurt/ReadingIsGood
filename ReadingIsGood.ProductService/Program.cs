using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using ReadingIsGood.Shared;

namespace ReadingIsGood.ProductService
{
    internal class Program
    {
        private static IEndpointInstance _productServiceEndpoint;
        private static IEndpointInstance _productServiceCallbacksEndpoint;
        public static ILogger Logger;

        private static void Main(string[] args)
        {
            //Mongo Database Password:a6KrZ3DOmUBvwSTz
            //https://cloud.mongodb.com/v2/5fdf9b8d4780eb4f6fe21924#security/database/users
            Logger = LoggingMechanism.CreateLogger("productService");
            InitializeEndpoint().GetAwaiter().GetResult();
        }

        private static async Task InitializeEndpoint()
        {
            try
            {
                _productServiceCallbacksEndpoint = await Endpoint
                    .Start(EndpointConfigurations.GetCallbackReceiverEndpointConfiguration())
                    .ConfigureAwait(false);

                _productServiceEndpoint = await Endpoint
                    .Start(EndpointConfigurations.GetProductServiceEndpointConfiguration())
                    .ConfigureAwait(false);


                while (true) Console.Read();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "ProductService catch error");
            }
            finally
            {
                await _productServiceEndpoint?.Stop();
                await _productServiceCallbacksEndpoint?.Stop();
            }
        }
    }
}