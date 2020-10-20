using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelloRin.Data;
using HelloRin.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rin.Core;
using Rin.Extensions.EntityFrameworkCore;

namespace HelloRin
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
            services.AddControllersWithViews()
                .AddRinMvcSupport();

            services.AddGrpc();

            // Register gRPC client for pseudo-client request.
            services.AddGrpcClient<Greeter.GreeterClient>(options =>
            {
                options.Address = new Uri("https://localhost:5001");
            });


            // Configure Entity Framework Core.
            services.AddDbContext<MyDbContext>(options =>
            {
                // NOTE: In-memory SQLite database must open a connection before use.
                var conn = new SqliteConnection("Data Source=MyDatabase;Mode=Memory;Cache=Shared");
                conn.Open();

                options.UseSqlite(conn);
            });

            services.AddRin(options =>
            {
                options.RequestRecorder.RetentionMaxRequests = 100;
                options.RequestRecorder.Excludes.Add(request => request.Path.Value.EndsWith(".js") || request.Path.Value.EndsWith(".css") || request.Path.Value.EndsWith(".svg"));
            })
                // Optional: Add Entity Framework Core support.
                .AddEntityFrameworkCoreDiagnostics()
                // Optional: Add a transformer for special data type.
                .AddBodyDataTransformer<RinCustomContentTypeTransformer>()
                // Optional: Use Redis as storage
                //.UseRedisStorage(options =>
                //{
                //    options.ConnectionConfiguration = "localhost:6379";
                //})
            ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseRin();
                app.UseRinMvcSupport();
                app.UseDeveloperExceptionPage();
                app.UseRinDiagnosticsHandler();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
