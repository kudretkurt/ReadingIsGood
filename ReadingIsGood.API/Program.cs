using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ReadingIsGood.Shared;

namespace ReadingIsGood.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(ApplicationConfiguration.Instance.GetValue<string>("ReadingIsGoodApi:Url"));
                });
        }
    }
}