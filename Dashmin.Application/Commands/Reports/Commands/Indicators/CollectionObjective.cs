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
    public class CollectionObjective : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        List<IndicatorResult> _model;

        /// <summary>
        /// Modelo de datos a procesar
        Organization _organization;
        public CollectionObjective(List<IndicatorResult> model,Organization organization)
        {
            _model = model;
            _organization = organization;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="CollectionObjective"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{CollectionObjective, Result}"/>
        /// </summary>
        class CollectionObjectiveHandeler : IRequestHandler<CollectionObjective, Result>
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
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="CollectionObjectiveHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="context"> Servicio que devuelve un contexto de la base de datos </param>
            public CollectionObjectiveHandeler(IConnectionService connection,IApplicationDBContext context)
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
            public async Task<Result> Handle(CollectionObjective request, CancellationToken cancellationToken)
            {
                var multiquery = string.Empty;
                string fechaDato = DateTime.Now.ToString("yyyy-MM-dd");
                int totalCounter = request._model.Count;
                Organization organization = request._organization;

                using (IDbConnection  conn = _connection.GetNpgsqlDb())
                {
                    List<ind_objetivo_cobranza> DataSet = new List<ind_objetivo_cobranza>();
                    int x = 0;
                    try
                    {
                        string sql = @$"INSERT INTO ind_objetivo_cobranza (numero_credito,empresa,fecha_documento,tipo_referencia,folio,cuenta,nombre_paciente,importe,pago,fecha_paquete,tipo_convenio,semana_objetivo,fecha_cobro,importe_cobro,semana_cobro,organizacion_id,mes_objetivo,empresa_contable,fecha_dato,bit_vencido)
                        VALUES (@numero_credito, @empresa, @fecha_documento, @tipo_referencia, @folio, @cuenta, @nombre_paciente, @importe, @pago, @fecha_paquete, @tipo_convenio, @semana_objetivo, @fecha_cobro, @importe_cobro, @semana_cobro, @organizacion_id, @mes_objetivo, @empresa_contable, @fecha_dato, @bit_vencido);";

                        DateTime dateValue;
                        foreach(IndicatorResult model in request._model)
                        {
                            try
                            {
                                ind_objetivo_cobranza data = new ind_objetivo_cobranza();
                                data.organizacion_id  = organization.IdOrganization;
                                data.fecha_dato       = DateTime.Parse(fechaDato);
                                data.empresa_contable = Int32.Parse(model.Business);
                                data.mes_objetivo     = Int32.Parse(model.IdIndicator);
                                data.numero_credito   = Int32.Parse(model.Value.Split('|')[0] != "" ? model.Value.Split('|')[0] : "0");
                                data.empresa          = model.Value.Split('|')[1]; 
                                data.fecha_documento  = DateTime.TryParse(model.Value.Split('|')[2], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.tipo_referencia  = model.Value.Split('|')[3]; 
                                data.folio            = model.Value.Split('|')[4]; 
                                data.cuenta           = Int32.Parse(model.Value.Split('|')[5] != "" ? model.Value.Split('|')[5] : "0");
                                data.nombre_paciente  = model.Value.Split('|')[6]; 
                                data.importe          = Int32.Parse(model.Value.Split('|')[7] != "" ? model.Value.Split('|')[7] : "0");
                                data.pago             = Int32.Parse(model.Value.Split('|')[8] != "" ? model.Value.Split('|')[8] : "0");
                                data.fecha_paquete    = DateTime.TryParse(model.Value.Split('|')[9], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.tipo_convenio    = model.Value.Split('|')[10]; 
                                data.semana_objetivo  = model.Value.Split('|')[11]; 
                                data.fecha_cobro      = DateTime.TryParse(model.Value.Split('|')[12], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.importe_cobro    = Int32.Parse(model.Value.Split('|')[13] != "" ? model.Value.Split('|')[13] : "0");
                                data.semana_cobro     = model.Value.Split('|')[14]; 
                                data.bit_vencido      = Int32.Parse(model.Value.Split('|')[15] != "" ? model.Value.Split('|')[15] : "0");
                                DataSet.Add(data);
                                x++;
                            }
                            catch(System.Exception ex)
                            {
                                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_saldos_bancos: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                            }
                        }
                        var deleteQuery = @$"DELETE FROM ind_objetivo_cobranza WHERE organizacion_id = {organization.IdOrganization} AND fecha_documento between '{DataSet.Min(o =>o.fecha_documento).ToString("yyyy-MM-dd")}' and '{DataSet.Max(o =>o.fecha_documento).ToString("yyyy-MM-dd")}'";
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        var affectedRows = conn.Execute( deleteQuery,commandType: CommandType.Text,commandTimeout: 900);

                        _= _context.ind_objetivo_cobranza.AddRangeAsync(DataSet);
                        await _context.SaveChangesAsync(cancellationToken);

                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - ind_objetivo_cobranza: Complete insert registers {totalCounter} \n");
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - ind_objetivo_cobranza: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                        return Result.Failure(new[]{ ex.Message } );
                    }

                    return Result.Success();
                }
            }
        }
    }
}