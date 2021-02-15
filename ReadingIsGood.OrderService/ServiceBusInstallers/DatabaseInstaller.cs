using System;
using System.Threading.Tasks;
using NServiceBus.Installation;

namespace ReadingIsGood.OrderService.ServiceBusInstallers
{
    public class DatabaseInstaller : INeedToInstallSomething
    {
        public Task Install(string identity)
        {
            Console.WriteLine("Installer executed");
            return Task.CompletedTask;
        }
    }
}