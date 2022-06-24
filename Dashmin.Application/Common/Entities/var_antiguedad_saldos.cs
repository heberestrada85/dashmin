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
    public class var_antiguedad_saldos
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public DateTime fecha_dato { get; set; }
        public long organizacion_id { get; set; }
        public string nombrecliente { get; set; }
        public DateTime fecha { get; set; }
        public long diasvencido { get; set; }
        public DateTime fechavence { get; set; }
        public string tipocredito { get; set; }
        public string folio { get; set; }
        public string siniestropaciente { get; set; }
        public string nombrepaciente { get; set; }
        public float importecredito { get; set; }
        public float importepagado { get; set; }
        public float saldo { get; set; }
        public string rango1 { get; set; }
        public string rango1nombre { get; set; }
        public long rango1activo { get; set; }
        public string rango2 { get; set; }
        public string rango2nombre { get; set; }
        public long rango2activo { get; set; }
        public string rango3 { get; set; }
        public string rango3nombre { get; set; }
        public long rango3activo { get; set; }
        public string rango4 { get; set; }
        public string rango4nombre { get; set; }
        public long rango4activo { get; set; }
        public string rango5 { get; set; }
        public string rango5nombre { get; set; }
        public long rango5activo { get; set; }
        public string anticipo { get; set; }
        public string anticiposin { get; set; }
        public long intnumcliente { get; set; }
        public string uuid { get; set; }
        public string agrupacion { get; set; }
        public float empresa_contable { get; set; }
    }
}