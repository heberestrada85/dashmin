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
using Hangfire;
using AutoMapper;
using System.Data;
using Dapper.Oracle;
using Newtonsoft.Json;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Dashmin.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Dashmin.Application.Common.Interface;
using System.Linq;

namespace Dashmin.Application.Reports.Commands
{
    /// <summary>
    /// Clase que se encarga de hacer la peticion de calcular el número de clientes activos dentro de la plaza en la semana actual.
    /// Implementa la interfaz <see cref="IRequest{List<IndicatorStatus>}"/>
    /// </summary>
    public class GetIndicatorStatus : IRequest<List<IndicatorStatus>>
    {
        /// <summary>
        /// Clase que se encarga de realizar la consulta de la base de datos a petición de  <see cref="GetIndicatorStatus"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{GetIndicatorStatus, List<IndicatorStatus>}"/>
        /// </summary>
        class GetIndicatorStatusHandeler : IRequestHandler<GetIndicatorStatus, List<IndicatorStatus>>
        {
            /// <summary>
            /// Indicadores
            /// </summary>
            private const string queryIndicador = @"SELECT
                                                        o.id AS Business,
                                                        o.siglas AS BusinessShortName,
                                                        o.nombre AS BusinessName,
                                                        i.nombre AS Indicator,
                                                        ia.id_indicador AS IdIndicator,
                                                        ia.fecha_actualizacion AS UpdateDate,
                                                        ia.fecha_inicio_indicador AS BeginDate,
                                                        ia.fecha_fin_indicador AS EndDate
                                                    FROM indicador_actualizacion ia
                                                    JOIN organizacion o ON o.id = ia.id_organizacion
                                                    LEFT JOIN indicador i ON i.id = ia.id_indicador
                                                    ORDER BY o.id, ia.id_indicador, ia.fecha_actualizacion";

            /// <summary>
            /// Referencia al servicio que devuelve una conexión a la base de datos
            /// </summary>
            IConnectionService _connection;

            /// <summary>
            /// IMediator
            /// </summary>
            ILogger _logger;

            /// <summary>
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="GetIndicatorStatusHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="apiService">IApiConnectorService.</param>
            /// <param name="mapper">IMapper</param>
            /// <param name="configuration">IConfiguration.</param>
            /// <param name="mediator">IMediator.</param>
            public GetIndicatorStatusHandeler(IConnectionService connection,ILogger<GetIndicatorStatusHandeler> logger)
            {
                _connection = connection;
                _logger = logger;
            }

            /// <summary>
            /// Función que se encarga de realmente realizar la consulta SQL y devolver la información mappeada a travez de dapper
            /// <see cref="Dapper"/>
            /// </summary>
            /// <param name="request"> Petición que origino esta consulta </param>
            /// <param name="cancellationToken"> Token de cancelación de las peticion asincrona </param>
            /// <returns> Devuelve una promesa que debe resolver un valor entero </returns>
            public async Task<List<IndicatorStatus>> Handle(GetIndicatorStatus request, CancellationToken cancellationToken)
            {
                using (var con = _connection.GetNpgsqlDb())
                {
                    IEnumerable<IndicatorStatus> queryResult = await con.QueryAsync<IndicatorStatus>(queryIndicador, new { }, null, 6000);

                    return queryResult.ToList();
                }
            }
        }
    }
}
