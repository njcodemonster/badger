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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using badgerApi.Interfaces;
using badgerApi.Helper;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using GenericModals;
using badgerApi.Helpers;

namespace badgerApi
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

            services.AddTransient<INotesAndDocHelper, NotesAndDocHelper>();
            services.AddTransient<IItemServiceHelper, ItemsServiceHelper>();
            services.AddTransient<IVendorRepository, VendorRepo>();
            services.AddTransient<IProductRepository, ProductRepo>();
            services.AddTransient<IPurchaseOrderStatusRepository, PurchaseOrderStatusRepo>();
            services.AddTransient<IAttributesRepository, AttributesRepo>();
            services.AddTransient<IAttributeTypeRepository, AttributeTypeRepo>();
            services.AddTransient<IAttributeValuesRepository, AttributeValuesRepo>();
            services.AddTransient<IPurchaseOrdersRepository, PurchaseOrdersRepo>();
            services.AddTransient<IPurchaseOrdersTrackingRepository, PurchaseOrdersTrackingRepo>();
            services.AddTransient<IPurchaseOrdersLedgerRepository, PurchaseOrdersLedgerRepo>();
            services.AddTransient<IPurchaseOrdersDiscountsRepository, PurchaseOrdersDiscountsRepo>();
            services.AddTransient<IVendorAddress, VendorAddressRepo>();
            services.AddTransient<IVendorRepRepository, VendorRepRepo>();
            services.AddTransient<IPhotoshootRepository, PhotoshootRepo>();
            services.AddTransient<IPhotoshootModelRepository, PhotoshootModelRepo>();
            services.AddTransient<IUserRepo, UserRepo>();
            services.AddTransient<ISkuRepo, SkuRepo>();
            services.AddTransient<IPurchaseOrdersLineItemsRepo, PurchaseOrdersLineItemsRepo>();
            services.AddTransient<IVendorProductRepository, VendorProductRepo>();
            services.AddTransient<iBarcodeRangeRepo, BarcodeRangeRepo>();
            services.AddTransient<IReportRepository, ReportsRepo>();
            services.AddTransient<IClaimRepository, ClaimRepository>();

            //* Singletons *\\
            services.AddSingleton<IProductCategoriesRepository, ProductCategoriesRepo>();
            services.AddSingleton<ICategoryRepository, CategoryRepo>();
            services.AddSingleton<IEventRepo, EventsRepo>();

            //services.Configure<ApiBehaviorOptions>(options =>
            //{
            //    options.InvalidModelStateResponseFactory = ctx => new ValidationProblemDetailsResult();
            //});
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/BadgerAPIFunctional-{Date}.txt");
            if (env.IsDevelopment())
            {
               //  app.UseDeveloperExceptionPage();
                LogGloblaErrors(app, loggerFactory);
            }
            else
            {
                LogGloblaErrors(app, loggerFactory);
                //app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void LogGloblaErrors(IApplicationBuilder app,ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();
                    var logger = loggerFactory.CreateLogger("Api Logs");
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
