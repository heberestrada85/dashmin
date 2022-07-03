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
using System.IO;
using AutoMapper;
using System.Data;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dashmin.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Dashmin.Application.Common.Entities;
using Dashmin.Application.Common.Interface;

namespace Dashmin.Application.Reports.Commands
{
    /// <summary>
    /// Clase que se encarga de hacer la peticion de calcular el número de clientes activos dentro de la plaza en la semana actual.
    /// Implementa la interfaz <see cref="IRequest{Result}"/>
    /// </summary>
    public class UpdateDashboard : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        List<IndicatorResult> _model;
        public UpdateDashboard(List<IndicatorResult> model)
        {
            _model = model;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="UpdateDashboard"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{UpdateDashboard, Result}"/>
        /// </summary>
        class UpdateDashboardHandeler : IRequestHandler<UpdateDashboard, Result>
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
            /// The mapper
            /// </summary>
            private readonly IMapper _mapper;

            /// <summary>
            /// IConfiguration
            /// </summary>
            IConfiguration _configuration;

            /// <summary>
            /// IApiConnectorService
            /// </summary>
            IApiConnectorService _apiService;

            /// <summary>
            /// IApiConnectorService
            /// </summary>
            IMediator _mediator;

            /// <summary>
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="UpdateDashboardHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="mapper">The mapper.</param>
            /// <param name="configuration">IConfiguration.</param>
            public UpdateDashboardHandeler(
                IConnectionService connection,
                IApiConnectorService apiService, 
                IMapper mapper,IConfiguration configuration,
                IMediator mediator,
                IApplicationDBContext context
            )
            {
                _connection = connection;
                _configuration = configuration;
                _mapper = mapper;
                _apiService = apiService;
                _mediator = mediator;
                _context = context;
            }

            /// <summary>
            /// Función que se encarga de realmente realizar la consulta SQL y devolver la información mappeada a travez de dapper
            /// <see cref="Dapper"/>
            /// </summary>
            /// <param name="request"> Petición que origino esta consulta </param>
            /// <param name="cancellationToken"> Token de cancelación de las peticion asincrona </param>
            /// <returns> Devuelve una promesa que debe resolver un valor entero </returns>
            public async Task<Result> Handle(UpdateDashboard request, CancellationToken cancellationToken)
            {
                Organization organization = new Organization();
                Organization organization_find = new Organization();

                using (IDbConnection  conn = _connection.GetNpgsqlDb())
                {
                    try
                    {
                        organization_find = await conn.QuerySingleAsync<Organization>("SELECT id AS IdOrganization, nombre AS Name FROM ORGANIZACION WHERE RAZON_SOCIAL = @razon",
                                                                    new { razon = request._model.FirstOrDefault().BusinessName }, null, 6000);

                        organization = await conn.QuerySingleAsync<Organization>("SELECT id AS IdOrganization, nombre AS Name FROM ORGANIZACION WHERE RAZON_SOCIAL = @razon AND ACTIVO = 1",
                                                                    new { razon = request._model.FirstOrDefault().BusinessName }, null, 6000);

                        if (organization_find is null)
                        {
                            string razon = request._model.FirstOrDefault().BusinessName;
                            string fechaDato = DateTime.Now.ToString("yyyy-MM-dd");
                            var insertQuery = @$"INSERT INTO organizacion (id,nombre,fecha_creacion,fecha_actualizacion,razon_social,activo)
                                                    VALUES ( (SELECT MAX(id) + 1 from organizacion),{razon},{fechaDato},{fechaDato},{razon},1) )";

                            var affectedRows = conn.Execute( insertQuery,commandType: CommandType.Text,commandTimeout: 900);
                            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $">>>> Creando organizacion desactivada {request._model.FirstOrDefault().BusinessName}");
                        }
                        else if (organization is null)
                        {
                            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $">>>> No se puede recuperar la informacion para la empresa {request._model.FirstOrDefault().BusinessName}");
                            return Result.Failure(new[]{ $"No se puede recuperar la informacion para la empresa {request._model.FirstOrDefault().BusinessName}" });
                        }

                        indicador_actualizacion entitie = new indicador_actualizacion();
                        entitie.id_organizacion = organization.IdOrganization;
                        entitie.fecha_actualizacion = DateTime.Now;
                        entitie.fecha_indicador = DateTime.Now;

                        switch (request._model.FirstOrDefault().IdIndicator)
                        {
                            case "1001":
                                //Objetivo_cobranza
                                entitie.id_indicador = 1001;
                                await _mediator.Send(new CollectionObjective(request._model,organization));
                                break;
                            case "1002":
                                //Total_egresos
                                entitie.id_indicador = 1002;
                                await _mediator.Send(new TotalExpenses(request._model,organization));
                                break;
                            case "1003":
                                //Consultas_urgencias
                                entitie.id_indicador = 1003;
                                await _mediator.Send(new EmergencyConsultations(request._model,organization));
                                break;
                            case "1004":
                                //Dias_hospitalizacion
                                entitie.id_indicador = 1004;
                                await _mediator.Send(new DaysHospitalization(request._model,organization));
                                break;
                            case "1005":
                                //Ingresos_actuales
                                entitie.id_indicador = 1005;
                                await _mediator.Send(new CurrentIncome(request._model,organization));
                                break;
                            case "1006":
                                //Diagnosticos_al_egreso
                                entitie.id_indicador = 1006;
                                await _mediator.Send(new DiagnosesUponDischarge(request._model,organization));
                                break;
                            case "1007":
                                //Facturacion_tipo_paciente
                                entitie.id_indicador = 1007;
                                await _mediator.Send(new PatientTypeBilling(request._model,organization));
                                break;
                            case "1008":
                                //Cuentas_pacientes
                                entitie.id_indicador = 1008;
                                await _mediator.Send(new PatientAccounts(request._model,organization));
                                break;
                            case "1009":
                                //Cirugias_pacientes
                                entitie.id_indicador = 1009;
                                await _mediator.Send(new PatientSurgeries(request._model,organization));
                                break;
                            case "1010":
                                //Ocupacion_hospitalaria
                                entitie.id_indicador = 1010;
                                await _mediator.Send(new HospitalOccupation(request._model,organization));
                                break;
                            case "1011":
                                //Cronologico_facturas
                                entitie.id_indicador = 1011;
                                await _mediator.Send(new ChronologicalInvoices(request._model,organization));
                                break;
                            case "1012":
                                //Antiguedad_saldos
                                entitie.id_indicador = 1012;
                                await _mediator.Send(new BalancesSeniority(request._model,organization));
                                break;
                            case "1013":
                                //Paquete_cobranza
                                entitie.id_indicador = 1013;
                                await _mediator.Send(new CollectionPackage(request._model,organization));
                                break;
                            case "1014":
                                //Limite_credito
                                entitie.id_indicador = 1014;
                                await _mediator.Send(new CreditLimit(request._model,organization));
                                break;
                            case "1015":
                                //Saldos_bancos
                                entitie.id_indicador = 1015;
                                await _mediator.Send(new BankBalances(request._model,organization));
                                break;
                            case "1016":
                                //Cargos Diarios
                                entitie.id_indicador = 1016;
                                await _mediator.Send(new DirectCharges(request._model,organization));
                                break;
                            case "1017":
                                //Pacientes_ingresos_por_urgencias
                                entitie.id_indicador = 1017;
                                await _mediator.Send(new PatientAdmissions(request._model,organization));
                                break;
                            default:
                                var multiquery = string.Empty;
                                DateTime fechaDato = DateTime.Now;
                                var listQuerys = request._model.Select( data => @$"SELECT ins_indicador_info('{long.Parse(data.IdIndicator)}','{organization.IdOrganization}','{fechaDato.ToString("yyyy-MM-dd")}','{long.Parse(data.Clave)}','{data.Description}','{data.Value}','{long.Parse(data.Business)}','{long.Parse(data.Clave)}','{data.Description}','{data.Value}','{long.Parse(data.IdIndicator)}','{data.Description}','{data.Value}');");
                                int totalCounter = request._model.Count;
                                foreach(string query in listQuerys)
                                {
                                    _ = conn.QueryFirstAsync(query, commandType: CommandType.Text, commandTimeout: 10).Result;
                                }
                                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $">>>> Complete insert registers {totalCounter} \n");
                                break;
                        }

                        await _mediator.Send(new UpdateIndicatorStatus(entitie));
                    }
                    catch (System.Exception ex)
                    {
                        string indicatorName = string.Empty;
                        switch (request._model.FirstOrDefault().IdIndicator)
                        {
                            case "1001":
                                //Objetivo_cobranza
                                indicatorName = "Objetivo_cobranza";
                                break;
                            case "1002":
                                //Total_egresos
                                indicatorName = "Total_egresos";
                                break;
                            case "1003":
                                //Consultas_urgencias
                                indicatorName = "Consultas_urgencias";
                                break;
                            case "1004":
                                //Dias_hospitalizacion
                                indicatorName = "Dias_hospitalizacion";
                                break;
                            case "1005":
                                //Ingresos_actuales
                                indicatorName = "Ingresos_actuales";
                                break;
                            case "1006":
                                //Diagnosticos_al_egreso
                                indicatorName = "Diagnosticos_al_egreso";
                                break;
                            case "1007":
                                //Facturacion_tipo_paciente
                                indicatorName = "Facturacion_tipo_paciente";
                                break;
                            case "1008":
                                //Cuentas_pacientes
                                indicatorName = "Cuentas_pacientes";
                                break;
                            case "1009":
                                //Cirugias_pacientes
                                indicatorName = "Cirugias_pacientes";
                                break;
                            case "1010":
                                //Ocupacion_hospitalaria
                                indicatorName = "Ocupacion_hospitalaria";
                                break;
                            case "1011":
                                //Cronologico_facturas
                                indicatorName = "Cronologico_facturas";
                                break;
                            case "1012":
                                //Antiguedad_saldos
                                indicatorName = "Antiguedad_saldos";
                                break;
                            case "1013":
                                //Paquete_cobranza
                                indicatorName = "Paquete_cobranza";
                                break;
                            case "1014":
                                //Limite_credito
                                indicatorName = "Limite_credito";
                                break;
                            case "1015":
                                //Saldos_bancos
                                indicatorName = "Saldos_bancos";
                                break;
                            case "1016":
                                //Cargos Diarios
                                indicatorName = "Cargos_Diarios";
                                break;
                            case "1017":
                                //Pacientes_ingresos_por_urgencias
                                indicatorName = "Pacientes_ingresos_por_urgencias";
                                break;
                        }
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $" >>>> {request._model.FirstOrDefault().IdIndicator}-{indicatorName} Error {ex.Message} \n");
                        return Result.Failure(new[]{ ex.Message } );
                    }
                    return Result.Success();
                }
            }
        }
    }
}