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
    public class ind_objetivo_cobranza
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public long numero_credito { get; set; }
        public string empresa { get; set; }
        public DateTime fecha_documento { get; set; }
        public string tipo_referencia { get; set; }
        public string folio { get; set; }
        public long cuenta { get; set; }
        public string nombre_paciente { get; set; }
        public long importe { get; set; }
        public long pago { get; set; }
        public DateTime fecha_paquete { get; set; }
        public string tipo_convenio { get; set; }
        public string semana_objetivo { get; set; }
        public DateTime fecha_cobro { get; set; }
        public long importe_cobro { get; set; }
        public string semana_cobro { get; set; }
        public long organizacion_id { get; set; }
        public long mes_objetivo { get; set; }
        public long empresa_contable { get; set; }
        public DateTime fecha_dato { get; set; }
        public long bit_vencido { get; set; }
    }
}