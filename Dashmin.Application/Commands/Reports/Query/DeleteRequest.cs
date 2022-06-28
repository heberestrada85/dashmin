/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System;
using Dapper;
using MediatR;
using AutoMapper;
using System.Data;
using Dapper.Oracle;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dashmin.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Dashmin.Application.Common.Interface;
using Hangfire;
using System.Globalization;

namespace Dashmin.Application.Reports.Commands
{
    /// <summary>
    /// Clase que se encarga de hacer la peticion de calcular el número de clientes activos dentro de la plaza en la semana actual.
    /// Implementa la interfaz <see cref="IRequest{Result}"/>
    /// </summary>
    public class DeleteRequest : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        Indicator _indicator;
        public DeleteRequest(Indicator indicator)
        {
            _indicator = indicator;
        }
        
        /// <summary>
        /// Clase que se encarga de realizar la consulta de la base de datos a petición de  <see cref="DeleteRequest"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{DeleteRequest, Result}"/>
        /// </summary>
        class DeleteRequestHandeler : IRequestHandler<DeleteRequest, Result>
        {
            /// <summary>
            /// IConfiguration
            /// </summary>
            IConfiguration _configuration;

            /// <summary>
            /// IApiConnectorService
            /// </summary>
            IApiConnectorService _apiService;

            /// <summary>
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="DeleteRequestHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="mapper">The mapper.</param>
            /// <param name="configuration">IConfiguration.</param>
            public DeleteRequestHandeler(IConnectionService connection,IApiConnectorService apiService,  IMapper mapper,IConfiguration configuration)
            {
                _configuration = configuration;
                _apiService = apiService;
            }

            /// <summary>
            /// Función que se encarga de realmente realizar la consulta SQL y devolver la información mappeada a travez de dapper
            /// <see cref="Dapper"/>
            /// </summary>
            /// <param name="request"> Petición que origino esta consulta </param>
            /// <param name="cancellationToken"> Token de cancelación de las peticion asincrona </param>
            /// <returns> Devuelve una promesa que debe resolver un valor entero </returns>
            [MaximumConcurrentExecutions(1, 60, 3)]
            [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
            public async Task<Result> Handle(DeleteRequest request, CancellationToken cancellationToken)
            {
                string apiAddress = string.Empty;

                apiAddress = Environment.GetEnvironmentVariable("DASHMINSERVER");
                if (apiAddress == string.Empty)
                    apiAddress = _configuration.GetValue<string>("DashminServer");

                _ = await _apiService.GetDataFromApi<Indicator>($"{apiAddress}/data/DeleteInfoDashboard", request._indicator);
                return Result.Success();
            }
        }
    }
}
