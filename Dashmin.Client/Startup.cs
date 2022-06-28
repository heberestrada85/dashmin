/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using Hangfire;
using Hangfire.SQLite;
using System.Reflection;
using Dashmin.Application;
using Dashmin.Infraestructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System;
using Dashmin.Application.Reports.Commands;
using Solar.Hangfire.Interfaces;
using Solar.Hangfire.Helpers;

namespace Dashmin.Client
{
    /// <summary>
    /// Clase de inicio para la inyecccion de dependencias y servicios
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Contructor de la clase
        /// </summary>
        /// <param name="configuration">la configuracion que va a recibir el inicio de la applicaicon</param>
        /// <param name="environment"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        /// <summary>
        /// Propiedad para la obtencion de la confguracion.
        /// </summary>
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        readonly string MyAllowSpecificOrigins = "_CorsPolicy";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddHealthChecks();
            Console.WriteLine("---------------------------------------------\n"+Configuration["ConnectionStrings:SQLiteConnection"]);
            services.AddHttpContextAccessor();
            services.AddMvc().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddHangfire( configuration => 
                configuration.UseSQLiteStorage(
                    Configuration.GetConnectionString("SQLiteConnection"),
                    new SQLiteStorageOptions()
                )
            );

            services.AddApplication();
            services.AddInfrastructure(Configuration);
            services.AddMediatR(Assembly.GetExecutingAssembly());

            // Development
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                   builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IRecurringJobManager jobManager, IServiceProvider serviceProvider)
        {
            // Development
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            }

            AddJobs(app,jobManager);
            var options = new BackgroundJobServerOptions { WorkerCount = 1 };
            app.UseHangfireServer(options);
            app.UseHangfireDashboard("/dashmin");
            app.UseHealthChecks("/health");

            // CORS
            app.UseHsts();
            app.UseCors(MyAllowSpecificOrigins);

            // Authorizations
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            // Use default static files
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Routing
            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        /// <summary>
        /// Anexando trabajos a la pila de llamadas
        /// </summary>
        public IApplicationBuilder AddJobs(IApplicationBuilder app, IRecurringJobManager jobManager)
        {
            var mediator = app.ApplicationServices.GetRequiredService<IMediator>();
            CommandSender commandSender = new CommandSender(mediator);
            jobManager.RemoveIfExists("Send data to dashboard");
            jobManager.AddOrUpdate("Send data to dashboard", () => commandSender.SendJob(new Dashmin.Application.Reports.Commands.GetData()), Cron.MinuteInterval(1));
            return app;
        }

        /// <summary>
        /// Assembly Version
        /// </summary>
        /// <returns></returns>
        public string GetAssemblyVersion() =>
            typeof(Program).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
    }
}
