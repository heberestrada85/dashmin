﻿/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using Dapper;
using System;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dashmin.Application.Common.Models;
using Dashmin.Application.Common.Entities;
using Dashmin.Application.Common.Interface;
using System.Text.Json;
using System.Linq;

namespace Dashmin.Application.Reports.Commands
{
    /// <summary>
    /// Clase que se encarga de atender la solicitud de actualizacion
    /// Implementa la interfaz <see cref="IRequest{Result}"/>
    /// </summary>
    public class CurrentIncome : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        List<IndicatorResult> _model;

        /// <summary>
        /// Modelo de datos a procesar
        Organization _organization;
        public CurrentIncome(List<IndicatorResult> model,Organization organization)
        {
            _model = model;
            _organization = organization;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="CurrentIncome"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{CurrentIncome, Result}"/>
        /// </summary>
        class CurrentIncomeHandeler : IRequestHandler<CurrentIncome, Result>
        {
            /// <summary>
            /// Referencia al servicio que devuelve una conexión a la base de datos
            /// </summary>
            IConnectionService _connection;

            /// <summary>
            /// Referencia al servicio que devuelve un contexto de la base de datos
            /// </summary>
            IApplicationDBContext _context;

            /// <summary>
            /// IMediator
            /// </summary>
            IMediator _mediator;

            /// <summary>
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="CurrentIncomeHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="context"> Servicio que devuelve un contexto de la base de datos </param>
            public CurrentIncomeHandeler(IConnectionService connection,IApplicationDBContext context)
            {
                _connection = connection;
                _context = context;
            }

            /// <summary>
            /// Función que se encarga de realmente realizar la consulta SQL y devolver la información mappeada a travez de dapper
            /// <see cref="Dapper"/>
            /// </summary>
            /// <param name="request"> Petición que origino esta consulta </param>
            /// <param name="cancellationToken"> Token de cancelación de las peticion asincrona </param>
            /// <returns> Devuelve una promesa que debe resolver un valor entero </returns>
            public async Task<Result> Handle(CurrentIncome request, CancellationToken cancellationToken)
            {
                var multiquery = string.Empty;
                string fechaDato = DateTime.Now.ToString("yyyy-MM-dd");
                int totalCounter = request._model.Count;
                Organization organization = request._organization;

                using (IDbConnection  conn = _connection.GetNpgsqlDb())
                {
                    List<var_total_ingresos_actuales> DataSet = new List<var_total_ingresos_actuales>();
                    int x = 0;
                    try
                    {
                        string sql = @$"INSERT INTO var_total_ingresos_actuales (fecha_dato,empresa_contable,organizacion_id,totalinternamientonormal,totalambulatorio,totalprepago,totalinternofueurgencias,totalinternofueambulatorio,totalreciennacido,totalinternamientoexpres,totalcortaestancia,totalinternofuecortaestancia,totalcortaestanciafueambulatorio,totalinternamientonormalconcentrado,fecha_genera_dato,sexo,edad,clave_cuenta_paciente)
                        VALUES (@fecha_dato, @empresa_contable, @organizacion_id, @totalinternamientonormal, @totalambulatorio, @totalprepago, @totalinternofueurgencias, @totalinternofueambulatorio, @totalreciennacido, @totalinternamientoexpres, @totalcortaestancia, @totalinternofuecortaestancia, @totalcortaestanciafueambulatorio, @totalinternamientonormalconcentrado, @fecha_genera_dato, @sexo, @edad, @clave_cuenta_paciente);";

                        DateTime dateValue;
                        foreach(IndicatorResult model in request._model)
                        {
                            try
                            {
                                var_total_ingresos_actuales data = new var_total_ingresos_actuales();
                                data.fecha_dato                          = DateTime.Parse(fechaDato);
                                data.empresa_contable                    = Int32.Parse(model.Business);
                                data.organizacion_id                     = organization.IdOrganization;
                                data.totalinternamientonormal            = Int32.Parse(model.Value.Split('|')[0] != "" ? model.Value.Split('|')[0] : "0");
                                data.totalambulatorio                    = Int32.Parse(model.Value.Split('|')[1] != "" ? model.Value.Split('|')[1] : "0");
                                data.totalprepago                        = Int32.Parse(model.Value.Split('|')[2] != "" ? model.Value.Split('|')[2] : "0");
                                data.totalinternofueurgencias            = Int32.Parse(model.Value.Split('|')[3] != "" ? model.Value.Split('|')[3] : "0");
                                data.totalinternofueambulatorio          = Int32.Parse(model.Value.Split('|')[4] != "" ? model.Value.Split('|')[4] : "0");
                                data.totalreciennacido                   = Int32.Parse(model.Value.Split('|')[5] != "" ? model.Value.Split('|')[5] : "0");
                                data.totalinternamientoexpres            = Int32.Parse(model.Value.Split('|')[6] != "" ? model.Value.Split('|')[6] : "0");
                                data.totalcortaestancia                  = Int32.Parse(model.Value.Split('|')[7] != "" ? model.Value.Split('|')[7] : "0");
                                data.totalinternofuecortaestancia        = Int32.Parse(model.Value.Split('|')[8] != "" ? model.Value.Split('|')[8] : "0");
                                data.totalcortaestanciafueambulatorio    = Int32.Parse(model.Value.Split('|')[9] != "" ? model.Value.Split('|')[9] : "0");
                                data.totalinternamientonormalconcentrado = Int32.Parse(model.Value.Split('|')[10] != "" ? model.Value.Split('|')[10] : "0");
                                data.sexo                                = model.Value.Split('|')[11]; 
                                data.edad                                = Int32.Parse(model.Value.Split('|')[12] != "" ? model.Value.Split('|')[12] : "0");
                                data.fecha_genera_dato                   = DateTime.TryParse(model.Value.Split('|')[13], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.clave_cuenta_paciente               = Int32.Parse(model.Value.Split('|')[14] != "" ? model.Value.Split('|')[14] : "0");
                                DataSet.Add(data);
                                x++;
                            }
                            catch(System.Exception ex)
                            {
                                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_saldos_bancos: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                            }
                        }
                        var deleteQuery = @$"DELETE FROM var_total_ingresos_actuales WHERE organizacion_id = {organization.IdOrganization} AND fecha_genera_dato between '{DataSet.Min(o =>o.fecha_genera_dato).ToString("yyyy-MM-dd")}' and '{DataSet.Max(o =>o.fecha_genera_dato).ToString("yyyy-MM-dd")}'";
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        var affectedRows = conn.Execute( deleteQuery,commandType: CommandType.Text,commandTimeout: 900);

                        _= _context.var_total_ingresos_actuales.AddRangeAsync(DataSet);
                        await _context.SaveChangesAsync(cancellationToken);

                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_total_ingresos_actuales: Complete insert registers {totalCounter} \n");
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_total_ingresos_actuales: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                        return Result.Failure(new[]{ ex.Message } );
                    }

                    return Result.Success();
                }
            }
        }
    }
}