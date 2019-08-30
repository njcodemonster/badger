using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using itemService.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using GenericModals;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace itemService
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
            services.AddTransient<IItemTypeRepository,ItemTypeRepo >();
            services.AddTransient<ItemRepository, ItemRepo>();
            services.AddTransient<IItemStatusRepository, ItemStatusRepo>();
            services.AddTransient<IItemEventsRepository, ItemEventsRepo>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/BadgerServiceFunctional-{Date}.txt");
            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
                LogGloblaErrors(app, loggerFactory);
            }
            else
            {
                LogGloblaErrors(app, loggerFactory);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void LogGloblaErrors(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();
                    var logger = loggerFactory.CreateLogger("Item Service Api Logs");
                    logger.LogInformation("Error: ", exceptionHandlerPathFeature.Error.ToString());

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var responseObj = JsonConvert.SerializeObject(new ResponseModel
                    {
                        Status = HttpStatusCode.InternalServerError,
                        Message = exceptionHandlerPathFeature.Error.ToString()
                    });
                    await context.Response.WriteAsync(responseObj);
                });
            });
        }
    }
}
