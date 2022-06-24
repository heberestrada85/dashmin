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
    public class var_limite_credito
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public long empresa_contable { get; set; }
        public DateTime fecha_dato { get; set; }
        public long organizacion_id { get; set; }
        public long numcliente { get; set; }
        public string tipocliente { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public float limitecredito { get; set; }
        public float saldoscli { get; set; }
        public DateTime fechafinal { get; set; }
        public long diasvencimiento { get; set; }
    }
}