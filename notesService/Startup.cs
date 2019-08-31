using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GenericModals;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using notesService.Interfaces;
using Serilog;

namespace notesService
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddTransient<INotesRepository, NotesRepo>();
            services.AddTransient<IDocumentsRepository, DocumentsRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddFile(@"C:\inetpub\serilog\NotesAndDocApiLogs\logs-{Date}.txt");
            loggerFactory.AddSerilog();
            if (env.IsDevelopment())
            {
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
