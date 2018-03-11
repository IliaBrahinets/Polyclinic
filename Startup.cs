using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Polyclinic.Data;
using Polyclinic.Helpers;
using Polyclinic.Services;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;

namespace Polyclinic
{
    public class Startup
    {

        protected ILogger _logger { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddDbContext<PolyclinicContext>(options =>
                                                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            ConfigureLogging(env, loggerFactory);

            if (env.IsDevelopment())
            {   
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }


            app.UseSession();

            app.UseStaticFiles();

            var locale = Configuration["SiteLocale"];
            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo> { new CultureInfo(locale) },
                SupportedUICultures = new List<CultureInfo> { new CultureInfo(locale) },
                DefaultRequestCulture = new RequestCulture(locale)
            };
            app.UseRequestLocalization(localizationOptions);
      
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=SignIn}/{action=Index}");
            });
        }

        private void ConfigureLogging(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var logConfig = Configuration.GetSection("Logging");
  
            var fileConfig = logConfig.GetSection("File");

            var path = fileConfig.GetValue<string>("path");
            var logLevel = Enum.Parse<LogLevel>(fileConfig.GetValue<string>("Loglevel"));

            loggerFactory.AddFile(path, logLevel);
            
        }
    }
}
