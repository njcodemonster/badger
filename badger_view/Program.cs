using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using badger_view.Hubs;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace badger_view
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateWebHostBuilder(args).UseKestrel(options =>
            //{
            //    options.Listen(IPAddress.Any, 9001, builder =>
            //    {
            //        builder.UseHub<ClaimtHub>();
            //    });
            //}).Build().Run();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
