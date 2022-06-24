/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada
using System;

namespace Dashmin.Application.Common.Interface
{
    /// <summary>
    /// Interfaz para la fecha sea mock
    /// </summary>
    public interface IDateTime
    {
        /// <summary>
        /// Gets the now.
        /// </summary>
        /// <value>The now.</value>
        DateTime Now { get; }
    }
}
