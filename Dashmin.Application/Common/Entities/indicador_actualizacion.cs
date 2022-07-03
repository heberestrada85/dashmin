/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2022, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System;

namespace Dashmin.Application.Common.Entities
{
    public class indicador_actualizacion
    {
        public long id { get; set; }
        public long id_organizacion { get; set; }
        public DateTime fecha_actualizacion { get; set; }
        public long id_indicador { get; set; }
        public DateTime fecha_indicador { get; set; }
    }
}