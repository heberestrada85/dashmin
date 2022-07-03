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
    public class PatientAdmissions : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        List<IndicatorResult> _model;

        /// <summary>
        /// Modelo de datos a procesar
        Organization _organization;
        public PatientAdmissions(List<IndicatorResult> model,Organization organization)
        {
            _model = model;
            _organization = organization;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="PatientAdmissions"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{PatientAdmissions, Result}"/>
        /// </summary>
        class PatientAdmissionsHandeler : IRequestHandler<PatientAdmissions, Result>
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
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="PatientAdmissionsHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="context"> Servicio que devuelve un contexto de la base de datos </param>
            public PatientAdmissionsHandeler(IConnectionService connection,IApplicationDBContext context)
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
            public async Task<Result> Handle(PatientAdmissions request, CancellationToken cancellationToken)
            {
                var multiquery = string.Empty;
                string fechaDato = DateTime.Now.ToString("yyyy-MM-dd");
                int totalCounter = request._model.Count;
                Organization organization = request._organization;

                using (IDbConnection  conn = _connection.GetNpgsqlDb())
                {
                    List<var_pacientes_ingresos> DataSet = new List<var_pacientes_ingresos>();
                    int x = 0;
                    try
                    {
                        string sql = @$"INSERT INTO var_pacientes_ingresos (empresa_contable,fecha_dato,organizacion_id,fechaingreso,numnumcuenta,numcvepaciente,vchnumcuarto,nombrepaciente,chrsexo,edad,nombremedico,tipopaciente,diagnostico,chrtipoingreso,area,estadosalud,bitorden,procedencia2,tipoingreso,numexpediente,tipoconsulta)
                        VALUES (@empresa_contable, @fecha_dato, @organizacion_id, @fechaingreso, @numnumcuenta, @numcvepaciente, @vchnumcuarto, @nombrepaciente, @chrsexo, @edad, @nombremedico, @tipopaciente, @diagnostico, @chrtipoingreso, @area, @estadosalud, @bitorden, @procedencia2, @tipoingreso, @numexpediente, @tipoconsulta);";

                        DateTime dateValue;
                        foreach(IndicatorResult model in request._model)
                        {
                            try
                            {
                                var_pacientes_ingresos data = new var_pacientes_ingresos();
                                data.empresa_contable = Int32.Parse(model.Business);
                                data.fecha_dato       = DateTime.Parse(fechaDato);
                                data.organizacion_id  = organization.IdOrganization;
                                data.fechaingreso     = DateTime.TryParse(model.Value.Split('|')[0], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.numnumcuenta     = Int32.Parse(model.Value.Split('|')[1] != "" ? model.Value.Split('|')[1] : "0");
                                data.numcvepaciente   = Int32.Parse(model.Value.Split('|')[2] != "" ? model.Value.Split('|')[2] : "0");
                                data.vchnumcuarto     = model.Value.Split('|')[3];
                                data.nombrepaciente   = model.Value.Split('|')[4];
                                data.chrsexo          = model.Value.Split('|')[5];
                                data.edad             = model.Value.Split('|')[6];
                                data.nombremedico     = model.Value.Split('|')[7];
                                data.tipopaciente     = model.Value.Split('|')[8];
                                data.diagnostico      = model.Value.Split('|')[9];
                                data.chrtipoingreso   = model.Value.Split('|')[10];
                                data.area             = model.Value.Split('|')[11];
                                data.estadosalud      = model.Value.Split('|')[12];
                                data.bitorden         = Int32.Parse(model.Value.Split('|')[13] != "" ? model.Value.Split('|')[13] : "0");
                                data.procedencia2     = model.Value.Split('|')[14];
                                data.tipoingreso      = model.Value.Split('|')[15];
                                data.numexpediente    = Int32.Parse(model.Value.Split('|')[16] != "" ? model.Value.Split('|')[16] : "0");
                                data.tipoconsulta     = model.Value.Split('|')[17];
                                DataSet.Add(data);
                                x++;
                            }
                            catch(System.Exception ex)
                            {
                                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_saldos_bancos: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                            }
                        }
                        var deleteQuery = @$"DELETE FROM var_pacientes_ingresos WHERE organizacion_id = {organization.IdOrganization} AND fechaingreso between '{DataSet.Min(o =>o.fechaingreso).ToString("yyyy-MM-dd")}' and '{DataSet.Max(o =>o.fechaingreso).ToString("yyyy-MM-dd")}'";
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        var affectedRows = conn.Execute( deleteQuery,commandType: CommandType.Text,commandTimeout: 900);

                        _= _context.var_pacientes_ingresos.AddRangeAsync(DataSet);
                        await _context.SaveChangesAsync(cancellationToken);

                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_pacientes_ingresos: Complete insert registers {totalCounter} \n");
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_pacientes_ingresos: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                        return Result.Failure(new[]{ ex.Message } );
                    }

                    return Result.Success();
                }
            }
        }
    }
}