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
    public class var_total_diagnosticos_egreso
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public DateTime fecha_dato { get; set; }
        public long empresa_contable { get; set; }
        public long organizacion_id { get; set; }
        public long clave_diagnostico { get; set; }
        public string desc_diagnostico { get; set; }
        public long total_diagnostico { get; set; }
        public string codigo_cie10 { get; set; }
        public string descripcion_cie10 { get; set; }
        public long es_cesarea { get; set; }
        public long es_parto { get; set; }
        public DateTime fecha_genera_dato { get; set; }
        public long clave_cuenta_paciente { get; set; }
    }
}