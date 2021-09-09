using EventBus.Messages.Common;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ordering.API.EventBusConsumer;
using Ordering.Application;
using Ordering.Infrastructure;

namespace Ordering.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //registerin our service registration container
            //from Application and infrastucture layer.

            services.AddApplicationServices();
            services.AddInfrastructureServices(Configuration);

            //MassTransit-RabbitMQ Configuration for perform async communication
            services.AddMassTransit(config =>
            {
                config.AddConsumer<BasketCheckoutConsumer>();
                //the above code is added to configure mass transit to consume basket event

                config.UsingRabbitMq((ctx, cfg) =>
                { //rabbitmq uses ampq protocol instead of http protocol and config is amqp://username:password@servername:rabbitmq port.
                    //and we use configuration to get host address so that we can override it in docker env.
                    cfg.Host(Configuration["EventBusSettings:HostAddress"]);


                    cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
                    {
                        c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
                    });
                });
            });
            services.AddMassTransitHostedService();

            //General Configuration
            services.AddAutoMapper(typeof(Startup));
            //Registering BasketCheckoutConsumer methodq
            services.AddScoped<BasketCheckoutConsumer>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.API v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
