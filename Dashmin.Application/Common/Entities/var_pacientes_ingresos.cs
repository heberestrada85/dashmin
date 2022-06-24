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
    public class var_pacientes_ingresos
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public long empresa_contable { get; set; }
        public DateTime fecha_dato { get; set; }
        public long organizacion_id { get; set; }
        public DateTime fechaingreso { get; set; }
        public long numnumcuenta { get; set; }
        public long numcvepaciente { get; set; }
        public string vchnumcuarto { get; set; }
        public string nombrepaciente { get; set; }
        public string chrsexo { get; set; }
        public string edad { get; set; }
        public string nombremedico { get; set; }
        public string tipopaciente { get; set; }
        public string diagnostico { get; set; }
        public string chrtipoingreso { get; set; }
        public string area { get; set; }
        public string estadosalud { get; set; }
        public long bitorden { get; set; }
        public string procedencia2 { get; set; }
        public string tipoingreso { get; set; }
        public long numexpediente { get; set; }
        public string tipoconsulta { get; set; }
    }
}