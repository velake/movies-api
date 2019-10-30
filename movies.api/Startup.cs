using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using movies.api.Extensions;
using movies.api.Interfaces;
using movies.api.Services;


namespace movies.api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Register the Swagger generator
            services.AddSwaggerConfig();

            services.AddResponseCaching();

            services.AddTransient<IOmdbService, OmdbService>();
            services.AddTransient<IAggregationService, AggregationService>();
            services.AddTransient<IYoutubeService, YoutubeService>();
            services.AddTransient<IGuideboxService, GuideboxService>();

            services.AddHttpClient("omdb", c =>
            {
                c.BaseAddress = new Uri("http://www.omdbapi.com/");
            });

            services.AddHttpClient("guidebox", c =>
            {
                c.BaseAddress = new Uri("http://api-public.guidebox.com/v2/");
            });

            services.ConfigureCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movies API V1"); });

            app.UseCors("CorsPolicy");

            app.UseResponseCaching();

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
