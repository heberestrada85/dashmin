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
    public class var_cargos_diario
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public long organizacion_id { get; set; }
        public DateTime fecha_dato { get; set; }
        public int empresa_contable { get; set; }
        public string folio_doc { get; set; }
        public string tipo { get; set; }
        public string conceptofac { get; set; }
        public string desccargo { get; set; }
        public float cantidad { get; set; }
        public string unidadventa { get; set; }
        public float precio { get; set; }
        public float importe { get; set; }
        public float descuento { get; set; }
        public float subtotal { get; set; }
        public float iva { get; set; }
        public float total { get; set; }
        public float ultimocosto { get; set; }
        public float costoventa { get; set; }
        public string cuenta { get; set; }
        public string paciente { get; set; }
        public string tipopaciente { get; set; }
        public string personaguardo { get; set; }
        public string idpaquete { get; set; }
        public DateTime fechacargo { get; set; }
        public long bitpaquete { get; set; }
        public string descpaquete { get; set; }
        public long cantidadpaquetes { get; set; }
        public float preciopaquete { get; set; }

    }
}