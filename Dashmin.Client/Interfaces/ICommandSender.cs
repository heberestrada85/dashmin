/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada


using System.Threading.Tasks;

namespace Solar.Hangfire.Interfaces
{
    /// <summary>
    /// Interfaz del servicio que conecta el Mediator con Hangfire
    /// </summary>
    public interface ICommandSender
    {
        /// <summary>
        /// Funcion que se encarga enviar el comando
        /// </summary>
        /// <typeparam name="T">Tipo de comando</typeparam>
        /// <param name="command">Comando a proccesar</param>
        /// <returns>Task a procesar</returns>
        public Task<object> SendJob<T>(T command);
    }
}
