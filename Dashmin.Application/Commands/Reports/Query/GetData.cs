﻿/////////////////////////////////////////////////////////////////////////////////////////////////
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

namespace Dashmin.Application.Reports.Commands
{
    /// <summary>
    /// Clase que se encarga de hacer la peticion de calcular el número de clientes activos dentro de la plaza en la semana actual.
    /// Implementa la interfaz <see cref="IRequest{Result}"/>
    /// </summary>
    public class GetData : IRequest<Result>
    {
        /// <summary>
        /// Clase que se encarga de realizar la consulta de la base de datos a petición de  <see cref="GetData"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{GetData, Result}"/>
        /// </summary>
        class GetDataHandeler : IRequestHandler<GetData, Result>
        {
            /// <summary>
            /// Indicadores
            /// </summary>
            private const string queryIndicador = @"SELECT
                                                        PI.INTCVEPROCEDIMIENTO AS IdIndicator,
                                                        PI.VCHNOMBRE AS StoreProcedure,
                                                        EH.BITEXPORTAHISTORICO AS ExportHistoric,
                                                        (TO_CHAR(EH.DTMFECHAEXPORT, 'YYYY-MM-DD')) AS ExportationDate,
                                                        (TO_CHAR(EH.DTMFECHAINICIAL, 'YYYY-MM-DD')) AS BeginDate,
                                                        (TO_CHAR(EH.DTMFECHAFINAL, 'YYYY-MM-DD')) AS EndDate,
                                                        PI.BITMUESTRAPREVIA AS DaysMemory,
                                                        CASE WHEN DTMFECHAEXPORT IS NULL THEN 
                                                            1
                                                        ELSE
                                                            CASE WHEN(((SYSDATE-DTMFECHAEXPORT)* 1440)> 1) THEN 1 ELSE 0 END
                                                        END AS CanExport
                                                    FROM DBPROCEDIMIENTOINDICADOR PI
                                                    INNER JOIN DBEXPORTAHISTORICO EH ON PI.INTCVEPROCEDIMIENTO = EH.INTCVEPROCEDIMIENTO
                                                    INNER JOIN DBHORARIO H ON H.INTCVEPROCEDIMIENTO = EH.INTCVEPROCEDIMIENTO
                                                    WHERE PI.BITACTIVO = 1 AND (TO_CHAR(H.DTMHORA, 'HH24:MI')) = (SELECT TO_CHAR(SYSDATE, 'HH24:MI') ""NOW"" FROM DUAL)";

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
            /// </summary>n
            IConfiguration _configuration;

            /// <summary>
            /// IApiConnectorService
            /// </summary>
            IApiConnectorService _apiService;

            /// <summary>
            /// IMediator
            /// </summary>
            IMediator _mediator;

            /// <summary>
            /// IMediator
            /// </summary>
            ILogger _logger;

            /// <summary>
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="GetDataHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="apiService">IApiConnectorService.</param>
            /// <param name="mapper">IMapper</param>
            /// <param name="configuration">IConfiguration.</param>
            /// <param name="mediator">IMediator.</param>
            public GetDataHandeler(
                    IConnectionService connection,
                    IApiConnectorService apiService,
                    IMapper mapper,
                    IConfiguration configuration,
                    IMediator mediator,
                    ILogger<GetDataHandeler> logger
            )
            {
                _connection = connection;
                _configuration = configuration;
                _mapper = mapper;
                _apiService = apiService;
                _mediator = mediator;
                _logger = logger;
            }

            /// <summary>
            /// Función que se encarga de realmente realizar la consulta SQL y devolver la información mappeada a travez de dapper
            /// <see cref="Dapper"/>
            /// </summary>
            /// <param name="request"> Petición que origino esta consulta </param>
            /// <param name="cancellationToken"> Token de cancelación de las peticion asincrona </param>
            /// <returns> Devuelve una promesa que debe resolver un valor entero </returns>
            public async Task<Result> Handle(GetData request, CancellationToken cancellationToken)
            {
                using (var con = _connection.GetOracleDb())
                {
                    IEnumerable<Indicator> indicadores = await con.QueryAsync<Indicator>(queryIndicador, new { }, null, 6000);
                    List<Indicator> indicadoresLista = _mapper.Map<List<Indicator>>(indicadores);
                    bool resultUpdate = false;

                    string apiAddress = string.Empty;
                    apiAddress = Environment.GetEnvironmentVariable("DASHMINSERVER");
                    if ((apiAddress == "") || (apiAddress == null))
                        apiAddress = _configuration.GetValue<string>("DashminServer");

                    string businessName = string.Empty;
                    businessName = Environment.GetEnvironmentVariable("BUSINESSNAME");
                    if ((businessName == "") || (businessName == null))
                        businessName = _configuration.GetValue<string>("BusinessName");

                    _apiService.HttpMethodSelector("POST");

                    DateTime dateToDisplay = DateTime.Now;
                    string date = dateToDisplay.ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));

                    _logger.LogInformation($"{date} - Esperando.......");
                    if (indicadoresLista.Count > 0)
                    {
                        foreach (Indicator indicador in indicadoresLista)
                        {
                            try
                            {
                                OracleDynamicParameters queryParameters = new OracleDynamicParameters();
                                indicador.BusinessName = businessName;
                                IndicatorResult result;

                                List<IndicatorResult> listIndicators = new List<IndicatorResult>();

                                string fechaFinal = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                string fechaInicial = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                                if (indicador.ExportationDate == null)
                                    fechaInicial = indicador.BeginDate;
                                else
                                {
                                    DateTime initialDate = DateTime.ParseExact(indicador.ExportationDate, "yyyy-MM-dd",null);
                                    if (indicador.DaysMemory > 0)
                                        initialDate = initialDate.AddDays((indicador.DaysMemory * -1));
                                    fechaInicial = initialDate.ToString("yyyy-MM-dd");
                                }

                                if (indicador.ExportHistoric == true)
                                    fechaFinal = indicador.EndDate;

                                indicador.BeginDate = fechaInicial;
                                indicador.EndDate = fechaFinal;
                                queryParameters.Add("IN_FECHAINICIO", indicador.BeginDate);
                                queryParameters.Add("IN_FECHAFIN", indicador.EndDate);
                                queryParameters.Add(name: ":RC1", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                                if (indicador.CanExport)
                                {
                                    using (var reader = await con.ExecuteReaderAsync(indicador.StoreProcedure, queryParameters, null, 60000, CommandType.StoredProcedure))
                                    {
                                        while (reader.Read())
                                        {
                                            result = new IndicatorResult()
                                            {
                                                Business = reader["EMPRESA"].ToString(),
                                                BusinessName = businessName,
                                                Clave = reader["CLAVE"].ToString(),
                                                Description = reader["DESCRIPCION"].ToString(),
                                                IdIndicator = reader["IDINDICADOR"].ToString(),
                                                Value = reader["VALOR"].ToString()
                                            };
                                            listIndicators.Add(result);
                                        }
                                        if ( listIndicators.Count > 0 )
                                        {
                                            _logger.LogInformation($"Send registers to server {apiAddress}/data/UpdateDashboard >>>> {listIndicators.Count.ToString()} \n");
                                            var (resultApi,apiType) = await _apiService.GetDataFromApi<Result,List<IndicatorResult>>($"{apiAddress}/data/UpdateDashboard", listIndicators);
                                            resultUpdate = resultApi.Succeeded;
                                        }
                                    }
                                }

                                string sqlQueryUpdate = string.Empty;
                                if (resultUpdate)
                                    sqlQueryUpdate = $"UPDATE DBEXPORTAHISTORICO SET DTMFECHAEXPORT=TO_TIMESTAMP('{indicador.EndDate}', 'YYYY-MM-DD HH24:MI:SS') WHERE INTCVEPROCEDIMIENTO = {indicador.IdIndicator}";
                                else
                                    sqlQueryUpdate = @$"UPDATE DBEXPORTAHISTORICO SET
                                                        DTMFECHAEXPORT = null,
                                                        DTMFECHAINICIAL = TO_TIMESTAMP('{indicador.BeginDate}', 'YYYY-MM-DD HH24:MI:SS'),
                                                        DTMFECHAFINAL = TO_TIMESTAMP('{DateTime.Now}', 'YYYY-MM-DD HH24:MI:SS'),
                                                        BITEXPORTAHISTORICO = 1
                                                        WHERE INTCVEPROCEDIMIENTO = {indicador.IdIndicator}";

                                int rowsAffected = con.Execute(sqlQueryUpdate);
                            }
                            catch (System.Exception ex)
                            {
                                string dateError = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                var sqlQueryUpdate = @$"UPDATE DBEXPORTAHISTORICO SET
                                                        DTMFECHAEXPORT = null,
                                                        DTMFECHAINICIAL = TO_TIMESTAMP('{indicador.BeginDate}', 'YYYY-MM-DD HH24:MI:SS'),
                                                        DTMFECHAFINAL = TO_TIMESTAMP('{dateError}', 'YYYY-MM-DD HH24:MI:SS'),
                                                        BITEXPORTAHISTORICO = 1
                                                        WHERE INTCVEPROCEDIMIENTO = {indicador.IdIndicator}";

                                int rowsAffected = con.Execute(sqlQueryUpdate);
                                _logger.LogError(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"Error {indicador.StoreProcedure} -> {ex.Message} {ex.InnerException} \n");
                            }
                        }
                    }
                }
                return Result.Success();
            }
        }
    }
}
