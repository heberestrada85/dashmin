/////////////////////////////////////////////////////////////////////////////////////////////////
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
    public class HospitalOccupation : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        List<IndicatorResult> _model;

        /// <summary>
        /// Modelo de datos a procesar
        Organization _organization;
        public HospitalOccupation(List<IndicatorResult> model,Organization organization)
        {
            _model = model;
            _organization = organization;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="HospitalOccupation"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{HospitalOccupation, Result}"/>
        /// </summary>
        class HospitalOccupationHandeler : IRequestHandler<HospitalOccupation, Result>
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
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="HospitalOccupationHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="context"> Servicio que devuelve un contexto de la base de datos </param>
            public HospitalOccupationHandeler(IConnectionService connection,IApplicationDBContext context)
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
            public async Task<Result> Handle(HospitalOccupation request, CancellationToken cancellationToken)
            {
                var multiquery = string.Empty;
                string fechaDato = DateTime.Now.ToString("yyyy-MM-dd");
                int totalCounter = request._model.Count;
                Organization organization = request._organization;

                using (IDbConnection  conn = _connection.GetNpgsqlDb())
                {
                    List<var_ocupacion_hospitalaria> DataSet = new List<var_ocupacion_hospitalaria>();
                    int x = 0;
                    try
                    {
                        string sql = @$"INSERT INTO var_ocupacion_hospitalaria (fecha_dato,empresa_contable,organizacion_id,fecha_genera_dato,total_camas,total_camas_censables,total_camas_censables_ocupadas,total_camas_censables_disponible,porcentaje_ocupacion_camas_censables,total_camas_no_censables,total_camas_no_censables_ocupadas,total_camas_no_censables_disponible,porcentaje_ocupacion_camas_no_censables,area_hospital)
                        VALUES (@fecha_dato, @empresa_contable, @organizacion_id, @fecha_genera_dato, @total_camas, @total_camas_censables, @total_camas_censables_ocupadas, @total_camas_censables_disponible, @porcentaje_ocupacion_camas_censables, @total_camas_no_censables, @total_camas_no_censables_ocupadas, @total_camas_no_censables_disponible, @porcentaje_ocupacion_camas_no_censables, @area_hospital);";

                        DateTime dateValue;
                        foreach(IndicatorResult model in request._model)
                        {
                            try
                            {
                                var_ocupacion_hospitalaria data = new var_ocupacion_hospitalaria();
                                data.fecha_dato                              = DateTime.Parse(fechaDato);
                                data.empresa_contable                        = Int32.Parse(model.Business);
                                data.organizacion_id                         = organization.IdOrganization;
                                data.fecha_genera_dato                       = DateTime.TryParse(model.Value.Split('|')[0], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.area_hospital                           = model.Value.Split('|')[1]; 
                                data.total_camas                             = Int32.Parse(model.Value.Split('|')[2] != "" ? model.Value.Split('|')[2] : "0");
                                data.total_camas_censables                   = Int32.Parse(model.Value.Split('|')[3] != "" ? model.Value.Split('|')[3] : "0");
                                data.total_camas_censables_ocupadas          = Int32.Parse(model.Value.Split('|')[4] != "" ? model.Value.Split('|')[4] : "0");
                                data.total_camas_censables_disponible        = Int32.Parse(model.Value.Split('|')[5] != "" ? model.Value.Split('|')[5] : "0");
                                data.porcentaje_ocupacion_camas_censables    = float.Parse(model.Value.Split('|')[6] != "" ? model.Value.Split('|')[6] : "0");
                                data.total_camas_no_censables                = Int32.Parse(model.Value.Split('|')[7] != "" ? model.Value.Split('|')[7] : "0");
                                data.total_camas_no_censables_ocupadas       = Int32.Parse(model.Value.Split('|')[8] != "" ? model.Value.Split('|')[8] : "0");
                                data.total_camas_no_censables_disponible     = Int32.Parse(model.Value.Split('|')[9] != "" ? model.Value.Split('|')[9] : "0");
                                data.porcentaje_ocupacion_camas_no_censables = float.Parse(model.Value.Split('|')[10] != "" ? model.Value.Split('|')[10] : "0");
                                DataSet.Add(data);
                                x++;
                            }
                            catch(System.Exception ex)
                            {
                                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_saldos_bancos: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                            }
                        }
                        var deleteQuery = @$"DELETE FROM var_ocupacion_hospitalaria WHERE organizacion_id = {organization.IdOrganization} AND fecha_genera_dato between '{DataSet.Min(o =>o.fecha_genera_dato).ToString("yyyy-MM-dd")}' and '{DataSet.Max(o =>o.fecha_genera_dato).ToString("yyyy-MM-dd")}'";
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        var affectedRows = conn.Execute( deleteQuery,commandType: CommandType.Text,commandTimeout: 900);

                        _= _context.var_ocupacion_hospitalaria.AddRangeAsync(DataSet);
                        await _context.SaveChangesAsync(cancellationToken);

                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_ocupacion_hospitalaria: Complete insert registers {totalCounter} \n");
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_ocupacion_hospitalaria: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                        return Result.Failure(new[]{ ex.Message } );
                    }

                    return Result.Success();
                }
            }
        }
    }
}