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
    public class ind_ingresos_facturados
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public long organizacionid { get; set; }
        public long empresacontable { get; set; }
        public long clavetipopaciente { get; set; }
        public string descripciontipopaciente { get; set; }
        public float importetipopaciente { get; set; }
        public long claveexterna2 { get; set; }
        public string descripcionexterna2 { get; set; }
        public float cargosnormalesfacturados { get; set; }
        public float cargosnormalescancelados { get; set; }
        public float otroscargosfacturados { get; set; }
        public float otroscargoscancelados { get; set; }
        public float facturadirecta { get; set; }
        public float facturadirectacancelada { get; set; }
        public float paquetesfacturados { get; set; }
        public float paquetescancelados { get; set; }
        public float membresiasociosfacturada { get; set; }
        public float membresiasocioscancelada { get; set; }
        public DateTime fecha_dato { get; set; }
    }
}