/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using MediatR;
using System.Reflection;
using Dashmin.Infraestructure.Services;
using Dashmin.Application.Common.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dashmin.Infraestructure.Persistence;

namespace Dashmin.Infraestructure
{
    /// <summary>
    /// Inyeccion de dependencias, de acuerdo a lo necesario para que la capa de infrestructura funcione
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient<IDateTime, DateTimeService>();
            services.AddScoped<IApiConnectorService, ApiConnectorService>();
            services.AddScoped<IConnectionService, ConnectionService>();
            services.AddScoped<IApplicationDBContext, ApplicationDBContext>();
            return services;
        }
    }
}