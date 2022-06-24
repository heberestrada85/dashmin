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
    public class var_saldos_bancos
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public long empresa_contable { get; set; }
        public DateTime fecha_dato { get; set; }
        public long organizacion_id { get; set; }
        public long idkardex { get; set; }
        public string nombrebanco { get; set; }
        public string moneda { get; set; }
        public DateTime fechamovimiento { get; set; }
        public string tipomovimiento { get; set; }
        public string referencia { get; set; }
        public float cargo { get; set; }
        public float abono { get; set; }
    }
}