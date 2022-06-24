/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using NSwag;
using System.Net;
using System.Reflection;
using Dashmin.Application;
using Dashmin.Infraestructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Dashmin.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dashmin.Server
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
            services.AddDbContext<ApplicationDBContext>(opt => { opt.UseNpgsql(Configuration.GetConnectionString("PostgresConnectionString")); });
            services.AddApplication();
            services.AddInfrastructure(Configuration);
            services.AddHttpContextAccessor();
            services.AddMvc().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            
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

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("192.168.10.2"));
            });

            //// Register the Swagger services
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Dashmin";
                    document.Info.Description = $"Dashmin (Assembly Version {GetAssemblyVersion()})";
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "Development",
                        Email = "development@andjon.com",
                    };
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseForwardedHeaders( new ForwardedHeadersOptions{
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            
            // CORS
            app.UseHsts();
            app.UseCors(MyAllowSpecificOrigins);

            // HealthChecks
            app.UseHealthChecks("/health");
            
            // Use default static files
            app.UseDefaultFiles();
            app.UseStaticFiles();
            
            // Authorizations
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

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
        /// Assembly Version
        /// </summary>
        /// <returns></returns>
        public string GetAssemblyVersion() =>
            typeof(Program).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
    }
}
