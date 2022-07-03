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
    public class PatientAccounts : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        List<IndicatorResult> _model;

        /// <summary>
        /// Modelo de datos a procesar
        Organization _organization;
        public PatientAccounts(List<IndicatorResult> model,Organization organization)
        {
            _model = model;
            _organization = organization;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="PatientAccounts"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{PatientAccounts, Result}"/>
        /// </summary>
        class PatientAccountsHandeler : IRequestHandler<PatientAccounts, Result>
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
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="PatientAccountsHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="context"> Servicio que devuelve un contexto de la base de datos </param>
            public PatientAccountsHandeler(IConnectionService connection,IApplicationDBContext context)
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
            public async Task<Result> Handle(PatientAccounts request, CancellationToken cancellationToken)
            {
                var multiquery = string.Empty;
                string fechaDato = DateTime.Now.ToString("yyyy-MM-dd");
                int totalCounter = request._model.Count;
                Organization organization = request._organization;

                using (IDbConnection  conn = _connection.GetNpgsqlDb())
                {
                    List<var_cuenta_paciente> DataSet = new List<var_cuenta_paciente>();
                    int x = 0;
                    try
                    {
                        string sql = @$"INSERT INTO var_cuenta_paciente (fecha_dato,empresa_contable,organizacion_id,clave_cuenta_paciente,nombre_paciente,sexo_paciente,edad_paciente,clave_medico_responsable,nombre_medico_responsable,clave_especialidad_medico,nombre_especialidad_medico,fecha_genera_dato,codigo_postal,tipo_ingreso,clave_departamento,nombre_departamento,nombre_estado,nombre_ciudad,nombre_pais)
                        VALUES (@fecha_dato, @empresa_contable, @organizacion_id, @clave_cuenta_paciente, @nombre_paciente, @sexo_paciente, @edad_paciente, @clave_medico_responsable, @nombre_medico_responsable, @clave_especialidad_medico, @nombre_especialidad_medico, @fecha_genera_dato, @codigo_postal, @tipo_ingreso, @clave_departamento, @nombre_departamento, @nombre_estado, @nombre_ciudad, @nombre_pais);";

                        DateTime dateValue;
                        foreach(IndicatorResult model in request._model)
                        {
                            try
                            {
                                var_cuenta_paciente data = new var_cuenta_paciente();
                                data.fecha_dato                 = DateTime.Parse(fechaDato);
                                data.empresa_contable           = Int32.Parse(model.Business);
                                data.organizacion_id            = organization.IdOrganization;
                                data.clave_cuenta_paciente      = Int32.Parse(model.Value.Split('|')[0] != "" ? model.Value.Split('|')[0] : "0");
                                data.nombre_paciente            = model.Value.Split('|')[1];
                                data.sexo_paciente              = model.Value.Split('|')[2];
                                data.edad_paciente              = Int32.Parse(model.Value.Split('|')[3] != "" ? model.Value.Split('|')[3] : "0");
                                data.clave_medico_responsable   = Int32.Parse(model.Value.Split('|')[4] != "" ? model.Value.Split('|')[4] : "0");
                                data.nombre_medico_responsable  = model.Value.Split('|')[5];
                                data.clave_especialidad_medico  = Int32.Parse(model.Value.Split('|')[6] != "" ? model.Value.Split('|')[6] : "0");
                                data.nombre_especialidad_medico = model.Value.Split('|')[7];
                                data.fecha_genera_dato          = DateTime.TryParse(model.Value.Split('|')[8], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.nombre_pais                = model.Value.Split('|')[9];
                                if ( model.Value.Split('|').Count() > 13)
                                {
                                    data.nombre_estado              = model.Value.Split('|')[10];
                                    data.nombre_ciudad              = model.Value.Split('|')[11];
                                    data.codigo_postal              = model.Value.Split('|')[12];
                                    data.tipo_ingreso               = model.Value.Split('|')[13];

                                    if (model.Value.Split('|').ElementAtOrDefault(14) != null)
                                        data.clave_departamento     = Int32.Parse(model.Value.Split('|')[14] != "" ? model.Value.Split('|')[14] : "0");

                                    if (model.Value.Split('|').ElementAtOrDefault(15) != null)
                                        data.nombre_departamento    = model.Value.Split('|')[15];
                                }
                                else{
                                    data.tipo_ingreso               = model.Value.Split('|')[10];
                                    data.clave_departamento         = Int32.Parse(model.Value.Split('|')[11] != "" ? model.Value.Split('|')[11] : "0");
                                    data.nombre_departamento        = model.Value.Split('|')[12];
                                }
                                DataSet.Add(data);
                                x++;
                            }
                            catch(System.Exception ex)
                            {
                                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_saldos_bancos: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                            }
                        }
                        var deleteQuery = @$"DELETE FROM var_cuenta_paciente WHERE organizacion_id = {organization.IdOrganization} AND fecha_genera_dato between '{DataSet.Min(o =>o.fecha_genera_dato).ToString("yyyy-MM-dd")}' and '{DataSet.Max(o =>o.fecha_genera_dato).ToString("yyyy-MM-dd")}'";
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        var affectedRows = conn.Execute( deleteQuery,commandType: CommandType.Text,commandTimeout: 900);

                        _= _context.var_cuenta_paciente.AddRangeAsync(DataSet);
                        await _context.SaveChangesAsync(cancellationToken);

                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_cuenta_paciente: Complete insert registers {totalCounter} \n");
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_cuenta_paciente: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                        return Result.Failure(new[]{ ex.Message } );
                    }

                    return Result.Success();
                }
            }
        }
    }
}
