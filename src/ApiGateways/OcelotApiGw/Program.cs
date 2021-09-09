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
