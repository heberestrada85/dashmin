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
    public class CreditLimit : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        List<IndicatorResult> _model;

        /// <summary>
        /// Modelo de datos a procesar
        Organization _organization;
        public CreditLimit(List<IndicatorResult> model,Organization organization)
        {
            _model = model;
            _organization = organization;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="CreditLimit"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{CreditLimit, Result}"/>
        /// </summary>
        class CreditLimitHandeler : IRequestHandler<CreditLimit, Result>
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
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="CreditLimitHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="context"> Servicio que devuelve un contexto de la base de datos </param>
            public CreditLimitHandeler(IConnectionService connection,IApplicationDBContext context)
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
            public async Task<Result> Handle(CreditLimit request, CancellationToken cancellationToken)
            {
                var multiquery = string.Empty;
                string fechaDato = DateTime.Now.ToString("yyyy-MM-dd");
                int totalCounter = request._model.Count;
                Organization organization = request._organization;

                using (IDbConnection  conn = _connection.GetNpgsqlDb())
                {
                    List<var_limite_credito> DataSet = new List<var_limite_credito>();
                    int x = 0;
                    try
                    {
                        string sql = @$"INSERT INTO var_limite_credito (empresa_contable,fecha_dato,organizacion_id,numcliente,tipocliente,nombre,descripcion,limitecredito,saldoscli,fechafinal,diasvencimiento)
                        VALUES (@empresa_contable, @fecha_dato, @organizacion_id, @numcliente, @tipocliente, @nombre, @descripcion, @limitecredito, @saldoscli, @fechafinal, @diasvencimiento);";

                        DateTime dateValue;
                        foreach(IndicatorResult model in request._model)
                        {
                            try
                            {
                                var_limite_credito data = new var_limite_credito();
                                data.empresa_contable = Int32.Parse(model.Business);
                                data.fecha_dato       = DateTime.Parse(fechaDato);
                                data.organizacion_id  = organization.IdOrganization;
                                data.numcliente       = Int32.Parse(model.Value.Split('|')[0] != "" ? model.Value.Split('|')[0] : "0");
                                data.tipocliente      = model.Value.Split('|')[1]; 
                                data.nombre           = model.Value.Split('|')[2]; 
                                data.descripcion      = model.Value.Split('|')[3]; 
                                data.limitecredito    = float.Parse(model.Value.Split('|')[4] != "" ? model.Value.Split('|')[4] : "0");
                                data.saldoscli        = float.Parse(model.Value.Split('|')[5] != "" ? model.Value.Split('|')[5] : "0");
                                data.diasvencimiento  = Int32.Parse(model.Value.Split('|')[6] != "" ? model.Value.Split('|')[6] : "0");
                                DataSet.Add(data);
                                x++;
                            }
                            catch(System.Exception ex)
                            {
                                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_saldos_bancos: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                            }
                        }
                        var deleteQuery = @$"DELETE FROM var_limite_credito WHERE organizacion_id = {organization.IdOrganization} AND fecha_dato between '{DataSet.Min(o =>o.fecha_dato).ToString("yyyy-MM-dd")}' and '{DataSet.Max(o =>o.fecha_dato).ToString("yyyy-MM-dd")}'";
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        var affectedRows = conn.Execute( deleteQuery,commandType: CommandType.Text,commandTimeout: 900);

                        _= _context.var_limite_credito.AddRangeAsync(DataSet);
                        await _context.SaveChangesAsync(cancellationToken);

                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_limite_credito: Complete insert registers {totalCounter} \n");
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_limite_credito: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                        return Result.Failure(new[]{ ex.Message } );
                    }

                    return Result.Success();
                }
            }
        }
    }
}