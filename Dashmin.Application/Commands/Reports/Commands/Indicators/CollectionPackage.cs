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
    public class CollectionPackage : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        List<IndicatorResult> _model;

        /// <summary>
        /// Modelo de datos a procesar
        Organization _organization;
        public CollectionPackage(List<IndicatorResult> model,Organization organization)
        {
            _model = model;
            _organization = organization;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="CollectionPackage"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{CollectionPackage, Result}"/>
        /// </summary>
        class CollectionPackageHandeler : IRequestHandler<CollectionPackage, Result>
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
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="CollectionPackageHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="context"> Servicio que devuelve un contexto de la base de datos </param>
            public CollectionPackageHandeler(IConnectionService connection,IApplicationDBContext context)
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
            public async Task<Result> Handle(CollectionPackage request, CancellationToken cancellationToken)
            {
                var multiquery = string.Empty;
                string fechaDato = DateTime.Now.ToString("yyyy-MM-dd");
                int totalCounter = request._model.Count;
                Organization organization = request._organization;

                using (IDbConnection  conn = _connection.GetNpgsqlDb())
                {
                    List<var_paquete_cobranza> DataSet = new List<var_paquete_cobranza>();
                    int x = 0;
                    try
                    {
                        string sql = @$"INSERT INTO var_paquete_cobranza (empresa_contable,fecha_dato,organizacion_id,folioreferencia,tiporeferencia,fechamovimiento,numcliente,nombredes,fechainicial,fechafinal,cantidadcredito,fecha_envio,paquete,tipocliente)
                        VALUES (@empresa_contable, @fecha_dato, @organizacion_id, @folioreferencia, @tiporeferencia, @fechamovimiento, @numcliente, @nombredes, @fechainicial, @fechafinal, @cantidadcredito, @fecha_envio, @paquete, @tipocliente);";

                        DateTime dateValue;
                        foreach(IndicatorResult model in request._model)
                        {
                            try
                            {
                                var_paquete_cobranza data = new var_paquete_cobranza();
                                data.empresa_contable = Int32.Parse(model.Business);
                                data.fecha_dato       = DateTime.Parse(fechaDato);
                                data.organizacion_id  = organization.IdOrganization;
                                data.folioreferencia  = model.Value.Split('|')[0]; 
                                data.tiporeferencia   = model.Value.Split('|')[1]; 
                                data.fechamovimiento  = DateTime.TryParse(model.Value.Split('|')[2], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.numcliente       = Int32.Parse(model.Value.Split('|')[3] != "" ? model.Value.Split('|')[3] : "0");
                                data.nombredes        = model.Value.Split('|')[4]; 
                                data.fechainicial     = DateTime.TryParse(model.Value.Split('|')[5], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.fechafinal       = DateTime.TryParse(model.Value.Split('|')[6], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.cantidadcredito  = Int32.Parse(model.Value.Split('|')[7] != "" ? model.Value.Split('|')[7] : "0");
                                data.fecha_envio      = DateTime.TryParse(model.Value.Split('|')[8], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.paquete          = model.Value.Split('|')[9]; 
                                data.tipocliente      = model.Value.Split('|')[10]; 
                                DataSet.Add(data);
                                x++;
                            }
                            catch(System.Exception ex)
                            {
                                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_saldos_bancos: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                            }
                        }
                        var deleteQuery = @$"DELETE FROM var_paquete_cobranza WHERE organizacion_id = {organization.IdOrganization} AND fechamovimiento between '{DataSet.Min(o =>o.fechamovimiento).ToString("yyyy-MM-dd")}' and '{DataSet.Max(o =>o.fechamovimiento).ToString("yyyy-MM-dd")}'";
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        var affectedRows = conn.Execute( deleteQuery,commandType: CommandType.Text,commandTimeout: 900);

                        _= _context.var_paquete_cobranza.AddRangeAsync(DataSet);
                        await _context.SaveChangesAsync(cancellationToken);

                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_paquete_cobranza: Complete insert registers {totalCounter} \n");
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_paquete_cobranza: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                        return Result.Failure(new[]{ ex.Message } );
                    }

                    return Result.Success();
                }
            }
        }
    }
}