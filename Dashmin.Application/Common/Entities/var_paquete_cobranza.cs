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
    public class var_paquete_cobranza
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public long empresa_contable { get; set; }
        public DateTime fecha_dato { get; set; }
        public long organizacion_id { get; set; }
        public string folioreferencia { get; set; }
        public string tiporeferencia { get; set; }
        public DateTime fechamovimiento { get; set; }
        public long numcliente { get; set; }
        public string nombredes { get; set; }
        public DateTime fechainicial { get; set; }
        public DateTime fechafinal { get; set; }
        public long cantidadcredito { get; set; }
        public DateTime fecha_envio { get; set; }
        public string paquete { get; set; }
        public string tipocliente { get; set; }
    }
}