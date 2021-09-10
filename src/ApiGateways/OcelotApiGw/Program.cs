using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OcelotApiGw
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    //adding and configuring project to use multiple json files depending on hosting environment.

                    config.AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();


                    //the code below is added to add logging feature to this gateway service.
                }).ConfigureLogging((hostingContext, LoggingBuilder) =>
                {
                    LoggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    LoggingBuilder.AddConsole();
                    LoggingBuilder.AddDebug();
                });
    }
}
