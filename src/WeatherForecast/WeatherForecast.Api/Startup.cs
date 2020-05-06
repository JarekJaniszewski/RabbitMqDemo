using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherForecast.CommonData.RabbitQueue;

namespace WeatherForecast.Api
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
            services.AddMvc(setupAction =>
            {
                //JSON accept and return type  format
                var jsonInputFormatter =
                    setupAction.InputFormatters.OfType<NewtonsoftJsonInputFormatter>().FirstOrDefault();
                jsonInputFormatter?.SupportedMediaTypes.Add("application/json");
            }).SetCompatibilityVersion(CompatibilityVersion.Latest);

                services.AddControllers();

            // Rabbit Dependency injection
            services.AddSingleton(sp => RabbitHutch.CreateBus("localhost"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.415
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
