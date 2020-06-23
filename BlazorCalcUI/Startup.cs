using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorCalcUI.Data;
using DataAccessLibrary.Controllers;
using DataAccessLibrary.DataBase;
using Syncfusion.Blazor;

namespace BlazorCalcUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSyncfusionBlazor();

            // Create single instance for entire website
            services.AddSingleton<WeatherForecastService>();

            // Create instance for every call
            services.AddTransient<IMySqlDataAccess, MySqlDataAccess>();
            services.AddTransient<IPeopleData, PeopleData>();
            services.AddTransient<IWindowOptionController, WindowOptionController>();
            //services.AddTransient<IKozijnKleurController, KozijnKleurController>();
            services.AddTransient<IKozijnController, KozijnController>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjcyNDgzQDMxMzgyZTMxMmUzMGdycTZDa1AzdUY3V1hNSlQwaUljNkJITXFLSTBvd2tKbkpYSXkzU0NQVFE9");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
