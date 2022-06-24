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
    public class var_facturacion_tipo_paciente
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public DateTime fecha_dato { get; set; }
        public long empresa_contable { get; set; }
        public long organizacion_id { get; set; }
        public long clave_tipo_paciente { get; set; }
        public string descripcion_tipo_paciente { get; set; }
        public long clave_empresa_tipo_paciente { get; set; }
        public string descripcion_empresa_tipo_paciente { get; set; }
        public float descuento { get; set; }
        public float iva { get; set; }
        public float total { get; set; }
        public DateTime fecha_genera_dato { get; set; }
        public long clave_cuenta_paciente { get; set; }
    }
}