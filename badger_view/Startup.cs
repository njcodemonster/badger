using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using badger_view.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using badger_view.Hubs;
using Microsoft.AspNetCore.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using CommonHelper.Extensions;
using System.Net;
using Newtonsoft.Json;
using GenericModals;

namespace badger_view
{
    public class Startup
    {

        public Startup(ILoggerFactory logger, IConfiguration configuration)
        {
            Configuration = configuration;
            _logger = logger;
        }
        private readonly ILoggerFactory _logger;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IConfigurationSection sec = Configuration.GetSection("Services_LIVE");
            services.Configure<AppSettings>(sec);
            //services.Configure<AppSettings>(Configuration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
               // options.ClientTimeoutInterval = TimeSpan.FromSeconds(10);
              //  options.KeepAliveInterval = TimeSpan.FromSeconds(9);
            });
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
                options.LoginPath = "/auth/Dologin";
            });
            services.AddSession(opts =>
            {
                opts.Cookie.IsEssential = true; // make the session cookie Essential
            }); 
            services.AddHttpContextAccessor();
            services.AddTransient<ILoginHelper, LoginHelper>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // LogGloblaErrors(app);
            }
            else
            {
                LogGloblaErrors(app);
            }

            //app.UseHsts();


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();
            WebSocketOptions webSocketOptions = GetAllowedOrigins();
            app.UseWebSockets(webSocketOptions);
            app.UseSignalR(routes =>
            {
                routes.MapHub<ClaimtHub>("/claimHub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=PurchaseOrders}/{action=Index}/{id?}");
            });
        }

        private WebSocketOptions GetAllowedOrigins()
        {
            var webSocketOptions = new WebSocketOptions();
            var allowedOrigins = Configuration.GetSection("AllowedOrigins").AsEnumerable().Where(x => x.Value != null);
            foreach (var origin in allowedOrigins)
            {
                webSocketOptions.AllowedOrigins.Add(origin.Value);
            }

            return webSocketOptions;
        }

        private void LogGloblaErrors(IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();

                        var log = _logger.CreateLogger("Badger View Log");
                        log.LogInformation("Error: ", exceptionHandlerPathFeature.Error.ToString());
                        await ProductionError(context);
                });
            });
        }

        private static async Task ProductionError(HttpContext context)
        {
            if (context.Request.IsAjaxRequest())
            {
                var responseObj = JsonConvert.SerializeObject(new ResponseModel
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "something went wrong plz contact customer support."
                });
                await context.Response.WriteAsync(responseObj);
            }
            else
            {
                context.Response.Redirect("/home/error");
            }
        }
    }
}
