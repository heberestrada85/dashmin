using System.Net.Cache;
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
using System.Linq;
using System.Data;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dashmin.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Dashmin.Application.Common.Interface;
using Dashmin.Application.Common.Entities;

namespace Dashmin.Application.Reports.Commands
{
    /// <summary>
    /// Clase que se encarga de hacer la peticion de calcular el número de clientes activos dentro de la plaza en la semana actual.
    /// Implementa la interfaz <see cref="IRequest{Result}"/>
    /// </summary>
    public class UpdateIndicatorStatus : IRequest<Result>
    {
        /// <summary>
        /// Modelo de datos a procesar
        public indicador_actualizacion _entitie;
        public UpdateIndicatorStatus(indicador_actualizacion entitie)
        {
            _entitie = entitie;
        }

        /// <summary>
        /// Clase que se encarga de realizar la actualizacion de la base de datos a petición de  <see cref="UpdateIndicatorStatus"/>
        /// esta clase implementa la interfaz <see cref="IRequestHandle{UpdateIndicatorStatus, Result}"/>
        /// </summary>
        class UpdateIndicatorStatusHandeler : IRequestHandler<UpdateIndicatorStatus, Result>
        {
            /// <summary>
            /// Referencia al servicio que devuelve una conexión a la base de datos
            /// </summary>
            IConnectionService _connection;

            /// <summary>
            /// Referencia al contexto de EF que devuelve un contexto de la base de datos
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
            /// IMediator
            /// </summary>
            IMediator _mediator;

            /// <summary>
            /// Constructor cuya funcion es la de crear una nueva instancia de <see cref="UpdateIndicatorStatusHandeler"/>
            /// </summary>
            /// <param name="connection"> Servicio que devuelve una conexión a la base de datos </param>
            /// <param name="mapper">The mapper.</param>
            /// <param name="configuration">IConfiguration.</param>
            public UpdateIndicatorStatusHandeler(
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
            public async Task<Result> Handle(UpdateIndicatorStatus request, CancellationToken cancellationToken)
            {
                try
                {
                    indicador_actualizacion information = _context.indicador_actualizacion.FirstOrDefault( o => o.id_organizacion == request._entitie.id_organizacion &&  o.id_indicador == request._entitie.id_indicador );
                    if (information == null)
                        await _context.indicador_actualizacion.AddAsync(request._entitie);
                    else
                    {
                        information.fecha_actualizacion =  request._entitie.fecha_actualizacion;
                        information.fecha_indicador =  request._entitie.fecha_indicador;
                        _context.indicador_actualizacion.Update(information);
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $" >>>> Update indicator status {request._entitie.id_organizacion}-{request._entitie.id_indicador} Error {ex.Message} \n");
                }
                return Result.Success();
            }
        }
    }
}