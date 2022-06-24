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
    public class var_cronologico_facturas
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public DateTime fecha_dato { get; set; }
        public long empresa_contable { get; set; }
        public long organizacion_id { get; set; }
        public long clave_cuenta_paciente { get; set; }
        public string nombre_paciente_cliente { get; set; }
        public string tipo_paciente_cliente { get; set; }
        public string descripcion_empresa { get; set; }
        public string razon_social_empresa { get; set; }
        public float importe_gravado_factura { get; set; }
        public float importe_gravado_nota { get; set; }
        public float importe_no_gravado_factura { get; set; }
        public float importe_no_gravado_nota { get; set; }
        public float subtotal_gravado { get; set; }
        public float subtotal_no_gravado { get; set; }
        public float descuento_gravado { get; set; }
        public float descuento_no_gravado { get; set; }
        public float iva { get; set; }
        public float total { get; set; }
        public string forma_pago { get; set; }
        public string estado_documento { get; set; }
        public DateTime fecha_documento { get; set; }
        public DateTime fecha_genera_dato { get; set; }
        public DateTime fecha_primer_documento { get; set; }
        public string folio { get; set; }
        public long clave_medico { get; set; }
        public string nombre_medico { get; set; }
        public long clave_especialidad { get; set; }
        public string nombre_especialidad { get; set; }
        public long clave_departamento { get; set; }
        public string nombre_departamento { get; set; }
    }
}