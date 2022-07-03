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
    public class PatientTypeBilling : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        List<IndicatorResult> _model;

        /// <summary>
        /// Modelo de datos a procesar
        Organization _organization;
        public PatientTypeBilling(List<IndicatorResult> model,Organization organization)
        {
            _model = model;
            _organization = organization;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="PatientTypeBilling"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{PatientTypeBilling, Result}"/>
        /// </summary>
        class PatientTypeBillingHandeler : IRequestHandler<PatientTypeBilling, Result>
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
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="PatientTypeBillingHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="context"> Servicio que devuelve un contexto de la base de datos </param>
            public PatientTypeBillingHandeler(IConnectionService connection,IApplicationDBContext context)
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
            public async Task<Result> Handle(PatientTypeBilling request, CancellationToken cancellationToken)
            {
                var multiquery = string.Empty;
                string fechaDato = DateTime.Now.ToString("yyyy-MM-dd");
                int totalCounter = request._model.Count;
                Organization organization = request._organization;

                using (IDbConnection  conn = _connection.GetNpgsqlDb())
                {
                    List<var_facturacion_tipo_paciente> DataSet = new List<var_facturacion_tipo_paciente>();
                    int x = 0;
                    try
                    {
                        string sql = @$"INSERT INTO var_facturacion_tipo_paciente (fecha_dato,empresa_contable,organizacion_id,clave_tipo_paciente,descripcion_tipo_paciente,clave_empresa_tipo_paciente,descripcion_empresa_tipo_paciente,descuento,iva,total,fecha_genera_dato,clave_cuenta_paciente)
                        VALUES (@fecha_dato, @empresa_contable, @organizacion_id, @clave_tipo_paciente, @descripcion_tipo_paciente, @clave_empresa_tipo_paciente, @descripcion_empresa_tipo_paciente, @descuento, @iva, @total, @fecha_genera_dato, @clave_cuenta_paciente);";

                        DateTime dateValue;
                        foreach(IndicatorResult model in request._model)
                        {
                            try
                            {
                                var_facturacion_tipo_paciente data = new var_facturacion_tipo_paciente();
                                data.fecha_dato                        = DateTime.Parse(fechaDato);
                                data.empresa_contable                  = Int32.Parse(model.Business);
                                data.organizacion_id                   = organization.IdOrganization;
                                data.clave_tipo_paciente               = Int32.Parse(model.Value.Split('|')[0] != "" ? model.Value.Split('|')[0] : "0");
                                data.descripcion_tipo_paciente         = model.Value.Split('|')[1]; 
                                data.clave_empresa_tipo_paciente       = Int32.Parse(model.Value.Split('|')[2] != "" ? model.Value.Split('|')[2] : "0");
                                data.descripcion_empresa_tipo_paciente = model.Value.Split('|')[3]; 
                                data.descuento                         = float.Parse(model.Value.Split('|')[4] != "" ? model.Value.Split('|')[4] : "0" );
                                data.iva                               = float.Parse(model.Value.Split('|')[5] != "" ? model.Value.Split('|')[5] : "0" );
                                data.total                             = float.Parse(model.Value.Split('|')[6] != "" ? model.Value.Split('|')[6] : "0" );
                                data.fecha_genera_dato                 = DateTime.TryParse(model.Value.Split('|')[7], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.clave_cuenta_paciente             = Int32.Parse(model.Value.Split('|')[8] != "" ? model.Value.Split('|')[8] : "0");
                                DataSet.Add(data);
                                x++;
                            }
                            catch(System.Exception ex)
                            {
                                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_saldos_bancos: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                            }
                        }
                        var deleteQuery = @$"DELETE FROM var_facturacion_tipo_paciente WHERE organizacion_id = {organization.IdOrganization} AND fecha_genera_dato between '{DataSet.Min(o =>o.fecha_genera_dato).ToString("yyyy-MM-dd")}' and '{DataSet.Max(o =>o.fecha_genera_dato).ToString("yyyy-MM-dd")}'";
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        var affectedRows = conn.Execute( deleteQuery,commandType: CommandType.Text,commandTimeout: 900);

                        _= _context.var_facturacion_tipo_paciente.AddRangeAsync(DataSet);
                        await _context.SaveChangesAsync(cancellationToken);

                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_facturacion_tipo_paciente: Complete insert registers {totalCounter} \n");
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_facturacion_tipo_paciente: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                        return Result.Failure(new[]{ ex.Message } );
                    }
                                            
                    return Result.Success();
                }
            }
        }
    }
}