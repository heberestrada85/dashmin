/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using Dashmin.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Dashmin.Application.Common.Interface;
using Dapper;
using System;
using System.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;

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
                                                                    new { razon = request._model.First().BusinessName }, null, 6000);

                        organization = await conn.QuerySingleAsync<Organization>("SELECT id AS IdOrganization, nombre AS Name FROM ORGANIZACION WHERE RAZON_SOCIAL = @razon AND ACTIVO = 1",
                                                                    new { razon = request._model.First().BusinessName }, null, 6000);

                        if (organization_find is null)
                        {
                            string razon = request._model.First().BusinessName;
                            string fechaDato = DateTime.Now.ToString("yyyy-MM-dd");
                            var insertQuery = @$"INSERT INTO organizacion (id,nombre,fecha_creacion,fecha_actualizacion,razon_social,activo)
                                                    VALUES ( (SELECT MAX(id) + 1 from organizacion),{razon},{fechaDato},{fechaDato},{razon},1) )";

                            var affectedRows = conn.Execute( insertQuery,commandType: CommandType.Text,commandTimeout: 900);
                            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $">>>> Creando organizacion desactivada {request._model.First().BusinessName}");
                        }
                        else if (organization is null)
                        {
                            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $">>>> No se puede recuperar la informacion para la empresa {request._model.First().BusinessName}");
                            return Result.Failure(new[]{ $"No se puede recuperar la informacion para la empresa {request._model.First().BusinessName}" });
                        }

                        switch (request._model.First().IdIndicator)
                        {
                            case "1001":
                                //Objetivo_cobranza
                                await _mediator.Send(new CollectionObjective(request._model,organization));
                                break;
                            case "1002":
                                //Total_egresos
                                await _mediator.Send(new TotalExpenses(request._model,organization));
                                break;
                            case "1003":
                                //Consultas_urgencias
                                await _mediator.Send(new EmergencyConsultations(request._model,organization));
                                break;
                            case "1004":
                                //Dias_hospitalizacion
                                await _mediator.Send(new DaysHospitalization(request._model,organization));
                                break;
                            case "1005":
                                //Ingresos_actuales
                                await _mediator.Send(new CurrentIncome(request._model,organization));
                                break;
                            case "1006":
                                //Diagnosticos_al_egreso
                                await _mediator.Send(new DiagnosesUponDischarge(request._model,organization));
                                break;
                            case "1007":
                                //Facturacion_tipo_paciente
                                await _mediator.Send(new PatientTypeBilling(request._model,organization));
                                break;
                            case "1008":
                                //Cuentas_pacientes
                                await _mediator.Send(new PatientAccounts(request._model,organization));
                                break;
                            case "1009":
                                //Cirugias_pacientes
                                await _mediator.Send(new PatientSurgeries(request._model,organization));
                                break;
                            case "1010":
                                //Ocupacion_hospitalaria
                                await _mediator.Send(new HospitalOccupation(request._model,organization));
                                break;
                            case "1011":
                                //Cronologico_facturas
                                await _mediator.Send(new ChronologicalInvoices(request._model,organization));
                                break;
                            case "1012":
                                //Antiguedad_saldos
                                await _mediator.Send(new BalancesSeniority(request._model,organization));
                                break;
                            case "1013":
                                //Paquete_cobranza
                                await _mediator.Send(new CollectionPackage(request._model,organization));
                                break;
                            case "1014":
                                //Limite_credito
                                await _mediator.Send(new CreditLimit(request._model,organization));
                                break;
                            case "1015":
                                //Saldos_bancos
                                await _mediator.Send(new BankBalances(request._model,organization));
                                break;
                            case "1016":
                                //Cargos Diarios
                                await _mediator.Send(new DirectCharges(request._model,organization));
                                break;
                            case "1017":
                                //Pacientes_ingresos_por_urgencias
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
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $">>>> {request._model.First().IdIndicator} Error {ex.Message} \n");
                        return Result.Failure(new[]{ ex.Message } );
                    }
                    return Result.Success();
                }
            }
        }
    }
}