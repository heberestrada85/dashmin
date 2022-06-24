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
    public class ChronologicalInvoices : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        List<IndicatorResult> _model;

        /// <summary>
        /// Modelo de datos a procesar
        Organization _organization;
        public ChronologicalInvoices(List<IndicatorResult> model,Organization organization)
        {
            _model = model;
            _organization = organization;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="ChronologicalInvoices"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{ChronologicalInvoices, Result}"/>
        /// </summary>
        class ChronologicalInvoicesHandeler : IRequestHandler<ChronologicalInvoices, Result>
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
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="ChronologicalInvoicesHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="context"> Servicio que devuelve un contexto de la base de datos </param>
            public ChronologicalInvoicesHandeler(IConnectionService connection,IApplicationDBContext context)
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
            public async Task<Result> Handle(ChronologicalInvoices request, CancellationToken cancellationToken)
            {
                var multiquery = string.Empty;
                string fechaDato = DateTime.Now.ToString("yyyy-MM-dd");
                int totalCounter = request._model.Count;
                Organization organization = request._organization;

                using (IDbConnection  conn = _connection.GetNpgsqlDb())
                {
                    List<var_cronologico_facturas> DataSet = new List<var_cronologico_facturas>();
                    int x = 0;
                    try
                    {
                        string sql = @$"INSERT INTO var_cronologico_facturas (fecha_dato,empresa_contable,organizacion_id,clave_cuenta_paciente,nombre_paciente_cliente,tipo_paciente_cliente,descripcion_empresa,razon_social_empresa,importe_gravado_factura,importe_gravado_nota,importe_no_gravado_factura,importe_no_gravado_nota,subtotal_gravado,subtotal_no_gravado,descuento_gravado,descuento_no_gravado,iva,total,forma_pago,estado_documento,fecha_documento,fecha_genera_dato,fecha_primer_documento,folio,clave_medico,nombre_medico,clave_especialidad,nombre_especialidad)
                        VALUES (@fecha_dato, @empresa_contable, @organizacion_id, @clave_cuenta_paciente, @nombre_paciente_cliente, @tipo_paciente_cliente, @descripcion_empresa, @razon_social_empresa, @importe_gravado_factura, @importe_gravado_nota, @importe_no_gravado_factura, @importe_no_gravado_nota, @subtotal_gravado, @subtotal_no_gravado, @descuento_gravado, @descuento_no_gravado, @iva, @total, @forma_pago, @estado_documento, @fecha_documento, @fecha_genera_dato, @fecha_primer_documento, @folio, @clave_medico, @nombre_medico, @clave_especialidad, @nombre_especialidad);";

                        DateTime dateValue;
                        foreach(IndicatorResult model in request._model)
                        {
                            try
                            {
                                var_cronologico_facturas data = new var_cronologico_facturas();
                                data.fecha_dato                 = DateTime.Parse(fechaDato);
                                data.empresa_contable           = Int32.Parse(model.Business);
                                data.organizacion_id            = organization.IdOrganization;
                                data.clave_cuenta_paciente      = Int32.Parse(model.Value.Split('|')[0] != "" ? model.Value.Split('|')[0] : "0");
                                data.nombre_paciente_cliente    = model.Value.Split('|')[1]; 
                                data.tipo_paciente_cliente      = model.Value.Split('|')[2]; 
                                data.descripcion_empresa        = model.Value.Split('|')[3]; 
                                data.razon_social_empresa       = model.Value.Split('|')[4]; 
                                data.importe_gravado_factura    = float.Parse(model.Value.Split('|')[5] != "" ? model.Value.Split('|')[5] : "0" );
                                data.importe_gravado_nota       = float.Parse(model.Value.Split('|')[6] != "" ? model.Value.Split('|')[6] : "0" );
                                data.importe_no_gravado_factura = float.Parse(model.Value.Split('|')[7] != "" ? model.Value.Split('|')[7] : "0" );
                                data.importe_no_gravado_nota    = float.Parse(model.Value.Split('|')[8] != "" ? model.Value.Split('|')[8] : "0" );
                                data.subtotal_gravado           = float.Parse(model.Value.Split('|')[9] != "" ? model.Value.Split('|')[9] : "0" );
                                data.subtotal_no_gravado        = float.Parse(model.Value.Split('|')[10] != "" ? model.Value.Split('|')[10] : "0" );
                                data.descuento_gravado          = float.Parse(model.Value.Split('|')[11] != "" ? model.Value.Split('|')[11] : "0" );
                                data.descuento_no_gravado       = float.Parse(model.Value.Split('|')[12] != "" ? model.Value.Split('|')[12] : "0" );
                                data.iva                        = float.Parse(model.Value.Split('|')[13] != "" ? model.Value.Split('|')[13] : "0" );
                                data.total                      = float.Parse(model.Value.Split('|')[14] != "" ? model.Value.Split('|')[14] : "0" );
                                data.forma_pago                 = model.Value.Split('|')[15];
                                data.estado_documento           = model.Value.Split('|')[16]; 
                                data.fecha_documento            = DateTime.TryParse(model.Value.Split('|')[17], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.fecha_genera_dato          = DateTime.TryParse(model.Value.Split('|')[18], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.fecha_primer_documento     = DateTime.TryParse(model.Value.Split('|')[19], out dateValue) ? dateValue : DateTime.Parse(fechaDato);
                                data.folio                      = model.Value.Split('|')[20]; 
                                data.clave_medico               = Int32.Parse(model.Value.Split('|')[21] != "" ? model.Value.Split('|')[21] : "0");
                                data.nombre_medico              = model.Value.Split('|')[22]; 
                                data.clave_especialidad         = Int32.Parse(model.Value.Split('|')[23] != "" ? model.Value.Split('|')[23] : "0");
                                data.nombre_especialidad        = model.Value.Split('|')[24]; 
                                data.clave_departamento         = Int32.Parse(model.Value.Split('|')[25] != "" ? model.Value.Split('|')[25] : "0");
                                data.nombre_departamento        = model.Value.Split('|')[26]; 
                                DataSet.Add(data);
                                x++;
                            }
                            catch(System.Exception ex)
                            {
                                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_saldos_bancos: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                            }
                        }
                        var deleteQuery = @$"DELETE FROM var_cronologico_facturas WHERE organizacion_id = {organization.IdOrganization} AND fecha_genera_dato between '{DataSet.Min(o =>o.fecha_genera_dato).ToString("yyyy-MM-dd")}' and '{DataSet.Max(o =>o.fecha_genera_dato).ToString("yyyy-MM-dd")}'";
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        var affectedRows = conn.Execute( deleteQuery,commandType: CommandType.Text,commandTimeout: 900);

                        _= _context.var_cronologico_facturas.AddRangeAsync(DataSet);
                        await _context.SaveChangesAsync(cancellationToken);

                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_cronologico_facturas: Complete insert registers {totalCounter} \n");
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"{organization.IdOrganization} - {organization.Name} - var_cronologico_facturas: Error {ex.Message} {JsonSerializer.Serialize(request._model[x])} \n");
                        return Result.Failure(new[]{ ex.Message } );
                    }

                    return Result.Success();
                }
            }
        }
    }
}