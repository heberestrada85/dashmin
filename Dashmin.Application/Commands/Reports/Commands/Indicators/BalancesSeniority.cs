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
    public class BalancesSeniority : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        List<IndicatorResult> _model;

        /// <summary>
        /// Modelo de datos a procesar
        Organization _organization;
        public BalancesSeniority(List<IndicatorResult> model,Organization organization)
        {
            _model = model;
            _organization = organization;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="BalancesSeniority"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{BalancesSeniority, Result}"/>
        /// </summary>
        class BalancesSeniorityHandeler : IRequestHandler<BalancesSeniority, Result>
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
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="BalancesSeniorityHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="context"> Servicio que devuelve un contexto de la base de datos </param>
            public BalancesSeniorityHandeler(IConnectionService connection,IApplicationDBContext context)
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
            public async Task<Result> Handle(BalancesSeniority request, CancellationToken cancellationToken)
            {
                var multiquery = string.Empty;
                string fechaDato = DateTime.Now.ToString("yyyy-MM-dd");
                int totalCounter = request._model.Count;
                Organization organization = request._organization;

                using (IDbConnection  conn = _connection.GetNpgsqlDb())
                {                   
                    List<var_antiguedad_saldos> DataSet = new List<var_antiguedad_saldos>();
                    int x = 0;
                    try
                    {
                        string sql = @$"INSERT INTO var_antiguedad_saldos (fecha_dato,organizacion_id,nombrecliente,fecha,diasvencido,fechavence,tipocredito,folio,siniestropaciente,nombrepaciente,importecredito,importepagado,saldo,rango1,rango1nombre,rango1activo,rango2,rango2nombre,rango2activo,rango3,rango3nombre,rango3activo,rango4,rango4nombre,rango4activo,rango5,rango5nombre,rango5activo,anticipo,anticiposin,intnumcliente,uuid,agrupacion,empresa_contable)
                        VALUES (@fecha_dato, @organizacion_id, @nombrecliente, @fecha, @diasvencido, @fechavence, @tipocredito, @folio, @siniestropaciente, @nombrepaciente, @importecredito, @importepagado, @saldo, @rango1, @rango1nombre, @rango1activo, @rango2, @rango2nombre, @rango2activo, @rango3, @rango3nombre, @rango3activo, @rango4, @rango4nombre, @rango4activo, @rango5, @rango5nombre, @rango5activo, @anticipo, @anticiposin, @intnumcliente, @uuid, @agrupacion, @empresa_contable);";
                        DateTime dateValue;
                        foreach(IndicatorResult model in request._model)
                        {
                            try
                            {
                                var_antiguedad_saldos data = new var_antiguedad_saldos();
                                data.organizacion_id   = organization.IdOrganization;
                                data.empresa_contable  = Int32.Parse(model.Business);
                                data.fecha_dato        = DateTime.Parse(fechaDato);
                                data.nombrecliente     = model.Value.Split('|')[0]; 
                                data.fecha             = DateTime.TryParse(model.Value.Split('|')[1], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.diasvencido       = Int32.Parse(model.Value.Split('|')[2] != "" ? model.Value.Split('|')[2] : "0");
                                data.fechavence        = DateTime.TryParse(model.Value.Split('|')[3], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.tipocredito       = model.Value.Split('|')[4]; 
                                data.folio             = model.Value.Split('|')[5]; 
                                data.siniestropaciente = model.Value.Split('|')[6]; 
                                data.nombrepaciente    = model.Value.Split('|')[7]; 
                                data.importecredito    = float.Parse(model.Value.Split('|')[8] != "" ? model.Value.Split('|')[8] : "0");
                                data.importepagado     = float.Parse(model.Value.Split('|')[9] != "" ? model.Value.Split('|')[9] : "0");
                                data.saldo             = float.Parse(model.Value.Split('|')[10] != "" ? model.Value.Split('|')[10] : "0");
                                data.rango1            = model.Value.Split('|')[11]; 
                                data.rango1nombre      = model.Value.Split('|')[12]; 
                                data.rango1activo      = Int32.Parse(model.Value.Split('|')[13] != "" ? model.Value.Split('|')[13] : "0");
                                data.rango2            = model.Value.Split('|')[14]; 
                                data.rango2nombre      = model.Value.Split('|')[15]; 
                                data.rango2activo      = Int32.Parse(model.Value.Split('|')[16] != "" ? model.Value.Split('|')[16] : "0");
                                data.rango3            = model.Value.Split('|')[17]; 
                                data.rango3nombre      = model.Value.Split('|')[18]; 
                                data.rango3activo      = Int32.Parse(model.Value.Split('|')[19] != "" ? model.Value.Split('|')[19] : "0");
                                data.rango4            = model.Value.Split('|')[20]; 
                                data.rango4nombre      = model.Value.Split('|')[21]; 
                                data.rango4activo      = Int32.Parse(model.Value.Split('|')[22] != "" ? model.Value.Split('|')[22] : "0");
                                data.rango5            = model.Value.Split('|')[23]; 
                                data.rango5nombre      = model.Value.Split('|')[24]; 
                                data.rango5activo      = Int32.Parse(model.Value.Split('|')[25] != "" ? model.Value.Split('|')[25] : "0");
                                data.anticipo          = model.Value.Split('|')[26]; 
                                data.anticiposin       = model.Value.Split('|')[27]; 
                                data.intnumcliente     = Int32.Parse(model.Value.Split('|')[28] != "" ? model.Value.Split('|')[28] : "0");
                                data.uuid              = model.Value.Split('|')[29]; 
                                data.agrupacion        = model.Value.Split('|')[30]; 
                                DataSet.Add(data);
                                x++;
                            }
                            catch(System.Exception ex)
                            {
                                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_saldos_bancos: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                            }
                        }

                        var deleteQuery = @$"DELETE FROM var_antiguedad_saldos WHERE organizacion_id = {organization.IdOrganization} AND fecha between '{DataSet.Min(o =>o.fecha).ToString("yyyy-MM-dd")}' and '{DataSet.Max(o =>o.fecha).ToString("yyyy-MM-dd")}'";
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        var affectedRows = conn.Execute( deleteQuery,commandType: CommandType.Text,commandTimeout: 900);

                        _= _context.var_antiguedad_saldos.AddRangeAsync(DataSet);
                        await _context.SaveChangesAsync(cancellationToken);

                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_antiguedad_saldos: Complete insert registers {totalCounter} \n");
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_antiguedad_saldos: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                        return Result.Failure(new[]{ ex.Message } );
                    }

                    return Result.Success();
                }
            }
        }
    }
}