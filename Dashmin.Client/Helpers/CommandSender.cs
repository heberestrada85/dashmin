/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada


using System;
using MediatR;
using Hangfire;
using System.Threading;
using System.Threading.Tasks;
using Solar.Hangfire.Interfaces;
using Dashmin.Application.Common.Models;

namespace Solar.Hangfire.Helpers
{
    /// <summary>
    /// Servicio que conecta el Mediator con Hangfire
    /// </summary>
    public class CommandSender
    {
        /// <summary>
        /// Servicio de mediacion de commandos
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Servicio de mediacion de commandos
        /// </summary>
        /// <param name="mediator">Servicio de mediacion de commandos</param>
        public CommandSender(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Funcion que se encarga enviar el comando
        /// </summary>
        /// <typeparam name="T">Tipo de comando</typeparam>
        /// <param name="command">Comando a proccesar</param>
        /// <returns>Task a procesar</returns>
        //[MaximumConcurrentExecutions(1, 58, 1)]
        //[DisableConcurrentExecution(timeoutInSeconds: 58)]
        [AutomaticRetry(Attempts = 1)]
        public async Task<object> SendJob<T>(T command)
        {
            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = cancellationTokenSource.Token;
                return await _mediator.Send(command,cancellationToken);;
            }
            catch (Exception e){
                Console.Write($"{e.Message}\n");
                return Task.FromResult(0L);
            }
        }
    }
}
