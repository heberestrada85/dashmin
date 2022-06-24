/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using Dashmin.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Dashmin.Application.Common.Interface;
using Dapper;
using System;
using System.Data;
using System.Collections.Generic;

namespace Dashmin.Application.Reports.Commands
{
    /// <summary>
    /// Clase que se encarga de hacer la peticion de calcular el número de clientes activos dentro de la plaza en la semana actual.
    /// Implementa la interfaz <see cref="IRequest{Result}"/>
    /// </summary>
    public class DeleteDashboard : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        Indicator _model;
        public DeleteDashboard(Indicator model)
        {
            _model = model;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="DeleteDashboard"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{DeleteDashboard, Result}"/>
        /// </summary>
        class DeleteDashboardHandeler : IRequestHandler<DeleteDashboard, Result>
        {
            /// <summary>
            /// Referencia al servicio que devuelve una conexión a la base de datos
            /// </summary>
            IConnectionService _connection;

            /// <summary>
            /// The mapper
            /// </summary>
            private readonly IMapper _mapper;

            /// <summary>
            /// IConfiguration
            /// </summary>
            IConfiguration _configuration;

            /// <summary>
            /// IApiConnectorService
            /// </summary>
            IApiConnectorService _apiService;

            /// <summary>
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="DeleteDashboardHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="mapper">The mapper.</param>
            /// <param name="configuration">IConfiguration.</param>
            public DeleteDashboardHandeler(IConnectionService connection,IApiConnectorService apiService,  IMapper mapper,IConfiguration configuration)
            {
                _connection = connection;
                _configuration = configuration;
                _mapper = mapper;
                _apiService = apiService;
            }

            /// <summary>
            /// Función que se encarga de realmente realizar la consulta SQL y devolver la información mappeada a travez de dapper
            /// <see cref="Dapper"/>
            /// </summary>
            /// <param name="request"> Petición que origino esta consulta </param>
            /// <param name="cancellationToken"> Token de cancelación de las peticion asincrona </param>
            /// <returns> Devuelve una promesa que debe resolver un valor entero </returns>
            public async Task<Result> Handle(DeleteDashboard request, CancellationToken cancellationToken)
            {
                using (var con = _connection.GetNpgsqlDb())
                {
                    var data = request._model;
                    Organization organization  = new Organization();

                    try
                    {
                        organization = await con.QuerySingleAsync<Organization>("SELECT id AS IdOrganization FROM ORGANIZACION WHERE RAZON_SOCIAL = @razon", 
                                                                        new { razon = data.BusinessName }, null, 6000);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $">>>> No se puede recuperar la informacion para la empresa {data.BusinessName}");
                        return Result.Failure(new[]{ $"No se puede recuperar la informacion para la empresa {data.BusinessName}" });
                    }

                    DatesTables options = await con.QuerySingleAsync<DatesTables>("select tabla AS Table, campo_fecha AS DatesOption from indicador where id_sp = @indicador", 
                                                                        new { indicador = data.IdIndicator.ToString() }, null, 6000);    

                    try
                    {
                        var deleteQuery = $"DELETE FROM {options.Table} WHERE organizacion_id = {organization.IdOrganization} and {options.DatesOption} between '{data.BeginDate}' and '{data.EndDate}'";
                        var affectedRows = con.Execute( deleteQuery,commandType: CommandType.Text,commandTimeout: 900);
                    }
                    catch (System.Exception ex)
                    {
                        return Result.Failure(new[]{ ex.Message } );
                    }
                }

                return Result.Success();
            }
        }
    }
}