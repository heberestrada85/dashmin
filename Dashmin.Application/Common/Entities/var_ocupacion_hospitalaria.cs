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
    public class var_ocupacion_hospitalaria
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public DateTime fecha_dato { get; set; }
        public long empresa_contable { get; set; }
        public long organizacion_id { get; set; }
        public DateTime fecha_genera_dato { get; set; }
        public long total_camas { get; set; }
        public long total_camas_censables { get; set; }
        public long total_camas_censables_ocupadas { get; set; }
        public long total_camas_censables_disponible { get; set; }
        public float porcentaje_ocupacion_camas_censables { get; set; }
        public long total_camas_no_censables { get; set; }
        public long total_camas_no_censables_ocupadas { get; set; }
        public long total_camas_no_censables_disponible { get; set; }
        public float porcentaje_ocupacion_camas_no_censables { get; set; }
        public string area_hospital { get; set; }
    }
}