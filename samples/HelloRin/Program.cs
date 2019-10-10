using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HelloRin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure log4net
            log4net.Config.XmlConfigurator.Configure(log4net.LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo("log4net.config"));

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(config =>
                {
                    config.AddRinLogger();
                })
                .UseStartup<Startup>();
    }
}
