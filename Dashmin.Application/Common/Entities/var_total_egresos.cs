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
    public class var_total_egresos
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public long empresa_contable { get; set; }
        public DateTime fecha_dato { get; set; }
        public long totalinternamientonormal { get; set; }
        public long totalambulatorio { get; set; }
        public long totalprepago { get; set; }
        public long totalinternofueurgencias { get; set; }
        public long totalinternofueambulatorio { get; set; }
        public long totalreciennacido { get; set; }
        public long totalinternamientoexpres { get; set; }
        public long totalcortaestancia { get; set; }
        public long totalinternofuecortaestancia { get; set; }
        public long totalcortaestanciafueambulatorio { get; set; }
        public long totalinternamientonormalconcentrado { get; set; }
        public long organizacion_id { get; set; }
        public string sexo { get; set; }
        public long edad { get; set; }
        public DateTime fecha_genera_dato { get; set; }
        public long clave_cuenta_paciente { get; set; }
    }
}