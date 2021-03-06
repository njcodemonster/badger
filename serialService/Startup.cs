﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GenericModals;
using itemService_entity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace serialService
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
             services.AddDbContext<itemService_entity.Models.itemsdbContext>(options => options.UseMySQL(Configuration.GetConnectionString("ItemsDatabase")));
            //services.Add(new ServiceDescriptor(typeof(itemsdbContext), new itemsdbContext(Configuration.GetConnectionString("ItemsDatabase"))));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
           // loggerFactory.AddFile("Logs/ItemServiceEntity-{Date}.txt");
            if (env.IsDevelopment())
            {
                LogGloblaErrors(app, loggerFactory);
            }
            else
            {
                LogGloblaErrors(app, loggerFactory);
                app.UseHsts();
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
                    var logger = loggerFactory.CreateLogger("Item Service Entity Logs");
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
