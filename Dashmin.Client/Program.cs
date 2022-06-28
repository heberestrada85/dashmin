/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2012, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System;
using MediatR;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace Dashmin.Client
{
    /// <summary>
    /// Inicio de la applicacion
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Metodo principal de entrda a la ejecucion
        /// </summary>
        /// <param name="args"></param>
        public static async Task Main(string[] args)
        {
            IWebHost host = CreateWebHostBuilder(args).Build();
            bool debug = false;
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
            }

            foreach (var arg in args)
            {
                if(arg == "--debug")
                    debug = true;
            }

            if (Debugger.IsAttached || debug)
            {
                host.Run();
            }
            else
            {
                host.RunAsService();
            }
            
        }

        /// <summary>
        ///     Creador del Hosting y configuracion
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string port = "5008";
            foreach (var arg in args)
            {
                if(arg.Contains("--port"))
                {
                    string[] subs = arg.Split("=");
                    port = subs[1];
                }
            }

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, Builder) =>
                {
                    Builder
                    .AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                })
                .UseUrls($"http://*:{port}")
                .UseStartup<Startup>();
        }
    }
}