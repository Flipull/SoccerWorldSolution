using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SoccerWorld;
using System;

namespace SoccerWorldSignalR
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
            services.AddControllersWithViews();
            services.AddDbContext<SoccerWorldDatabaseContext>(options => 
            {
                options.UseSqlServer(Configuration.GetConnectionString("LocalSqlExpress"));
            });
            //services.AddScoped<SoccerWorldDatabaseContext>();
            services.AddTransient<SoccerWorldDatabaseContext>();

            services.AddSignalR().AddHubOptions<SignalRHub>(options => 
            {
                options.EnableDetailedErrors = true;
                //options.ClientTimeoutInterval = new TimeSpan(0, 1, 30);
                //options.KeepAliveInterval = new TimeSpan(0, 0, 30);
            });
            //services.AddTransient<SignalRHub>();
            //services.AddScoped<SignalRHub>();
            services.AddSingleton<SignalRHub>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<SignalRHub>("/hub");
            });
            
            var provider = app.ApplicationServices.CreateScope().ServiceProvider;
            SoccerWorldDatabaseContext._internal_serviceprovider = provider;
            SignalRHub._internal_serviceprovider = provider;



            using (var dbcontext = provider.GetService<SoccerWorldDatabaseContext>())
            {
                //Seed.RecreateDatabase(dbcontext);
                //Seed.SeedDatabase(dbcontext);
            };

        }
    }
}
