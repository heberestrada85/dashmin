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
    public class DirectCharges : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        List<IndicatorResult> _model;

        /// <summary>
        /// Modelo de datos a procesar
        Organization _organization;
        public DirectCharges(List<IndicatorResult> model,Organization organization)
        {
            _model = model;
            _organization = organization;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="DirectCharges"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{DirectCharges, Result}"/>
        /// </summary>
        class DirectChargesHandeler : IRequestHandler<DirectCharges, Result>
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
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="DirectChargesHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="context"> Servicio que devuelve un contexto de la base de datos </param>
            public DirectChargesHandeler(IConnectionService connection,IApplicationDBContext context)
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
            public async Task<Result> Handle(DirectCharges request, CancellationToken cancellationToken)
            {
                var multiquery = string.Empty;
                string fechaDato = DateTime.Now.ToString("yyyy-MM-dd");
                int totalCounter = request._model.Count;
                Organization organization = request._organization;

                using (IDbConnection  conn = _connection.GetNpgsqlDb())
                {
                    List<var_cargos_diario> DataSet = new List<var_cargos_diario>();
                    int x = 0;
                    try
                    {
                        DateTime dateValue;
                        foreach(IndicatorResult model in request._model)
                        {
                            try
                            {
                                var_cargos_diario data = new var_cargos_diario();
                                data.organizacion_id    = organization.IdOrganization;
                                data.fecha_dato         = DateTime.Parse(fechaDato);
                                data.empresa_contable   = Int32.Parse(model.Business);
                                data.folio_doc          = model.Value.Split('|')[0];
                                data.tipo               = model.Value.Split('|')[1];
                                data.conceptofac        = model.Value.Split('|')[2];
                                data.desccargo          = model.Value.Split('|')[3];
                                data.cantidad           = float.Parse(model.Value.Split('|')[4] != "" ? model.Value.Split('|')[4] : "0" );
                                data.unidadventa        = model.Value.Split('|')[5];
                                data.precio             = float.Parse(model.Value.Split('|')[6] != "" ? model.Value.Split('|')[6] : "0");
                                data.importe            = float.Parse(model.Value.Split('|')[7] != "" ? model.Value.Split('|')[7] : "0");
                                data.descuento          = float.Parse(model.Value.Split('|')[8] != "" ? model.Value.Split('|')[8] : "0");
                                data.subtotal           = float.Parse(model.Value.Split('|')[9] != "" ? model.Value.Split('|')[9] : "0");
                                data.iva                = float.Parse(model.Value.Split('|')[10] != "" ? model.Value.Split('|')[10] : "0");
                                data.total              = float.Parse(model.Value.Split('|')[11] != "" ? model.Value.Split('|')[11] : "0");
                                data.ultimocosto        = float.Parse(model.Value.Split('|')[12] != "" ? model.Value.Split('|')[12] : "0");
                                data.costoventa         = float.Parse(model.Value.Split('|')[13] != "" ? model.Value.Split('|')[13] : "0");
                                data.cuenta             = model.Value.Split('|')[14];
                                data.paciente           = model.Value.Split('|')[15];
                                data.tipopaciente       = model.Value.Split('|')[16];
                                data.personaguardo      = model.Value.Split('|')[17];
                                data.idpaquete          = model.Value.Split('|')[18];
                                data.fechacargo         = DateTime.TryParse(model.Value.Split('|')[19], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.bitpaquete         = Int32.Parse(model.Value.Split('|')[20] != "" ? model.Value.Split('|')[20] : "0");
                                data.descpaquete        = model.Value.Split('|')[21];
                                data.cantidadpaquetes   = Int32.Parse(model.Value.Split('|')[22] != "" ? model.Value.Split('|')[22] : "0");
                                data.preciopaquete      = float.Parse(model.Value.Split('|')[23] != "" ? model.Value.Split('|')[23] : "0");
                                DataSet.Add(data);
                                x++;
                            }
                            catch(System.Exception ex)
                            {
                                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_saldos_bancos: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                            }
                        }
                        var deleteQuery = @$"DELETE FROM var_cargos_diario WHERE organizacion_id = {organization.IdOrganization} AND fechaCargo between '{DataSet.Min(o =>o.fechacargo).ToString("yyyy-MM-dd")}' and '{DataSet.Max(o =>o.fechacargo).ToString("yyyy-MM-dd")}'";
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        var affectedRows = conn.Execute( deleteQuery,commandType: CommandType.Text,commandTimeout: 900);

                        _context.var_cargos_diario.AddRange(DataSet);
                        await _context.SaveChangesAsync(cancellationToken);

                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_cargos_diario: Complete insert registers {totalCounter} \n");
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_cargos_diario: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                        return Result.Failure(new[]{ ex.Message } );
                    }

                    return Result.Success();
                }
            }
        }
    }
}