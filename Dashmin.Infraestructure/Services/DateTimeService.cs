/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System;
using Dashmin.Application.Common.Interface;

namespace Dashmin.Infraestructure.Services
{
    /// <summary>
    /// Servicio de fecha para implementar las fechas en una sola clase
    /// </summary>
    public class DateTimeService : IDateTime
    {
        /// <summary>
        /// Fecha actual del sistema.... dada por el servidor.
        /// </summary>
        public DateTime Now => DateTime.Now;
    }
}
