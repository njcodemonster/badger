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
            services.AddTransient<INotesAndDocHelper,NotesAndDocHelper>();
            services.AddTransient<IEventRepo, EventsRepo>();
            services.AddTransient<IVendorRepository, VendorRepo>();
            services.AddTransient<IProductRepository, ProductRepo>();
            services.AddTransient<IPurchaseOrderStatusRepository, PurchaseOrderStatusRepo>();
            services.AddTransient<IAttributesRepository, AttributesRepo>();
            services.AddTransient<IAttributeTypeRepository, AttributeTypeRepo>();
            services.AddTransient<IAttributeValuesRepository, AttributeValuesRepo>();
            services.AddTransient<IPurchaseOrdersRepository, PurchaseOrdersRepo>();
            services.AddTransient<IVendorAddress, VendorAddressRepo>();
            services.AddTransient<IVendorRepRepository, VendorRepRepo>();
            services.AddTransient<IPhotoshootRepository, PhotoshootRepo>();
            
            services.AddTransient<IPhotoshootModelRepository, PhotoshootModelRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/BadgerAPIFunctional-{Date}.txt");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
