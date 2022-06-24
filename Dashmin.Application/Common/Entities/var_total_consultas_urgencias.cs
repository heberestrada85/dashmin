/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System;

namespace Dashmin.Application.Common.Entities
{
    public class var_total_consultas_urgencias
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public DateTime fecha_dato { get; set; }
        public long empresa_contable { get; set; }
        public long organizacion_id { get; set; }
        public long totalurgencias { get; set; }
        public long totalexterno { get; set; }
        public long totalconsultaexterna { get; set; }
        public DateTime fecha_genera_dato { get; set; }
        public long clave_cuenta_paciente { get; set; }
    }
}