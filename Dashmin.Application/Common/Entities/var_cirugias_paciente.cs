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
    public class var_cirugias_paciente
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public DateTime fecha_dato { get; set; }
        public long empresa_contable { get; set; }
        public long organizacion_id { get; set; }
        public long clave_cuenta_paciente { get; set; }
        public long clave_cirujano_principal { get; set; }
        public string nombre_cirujano_principal { get; set; }
        public long clave_especialidad_cirujano { get; set; }
        public string nombre_especialidad_cirujano { get; set; }
        public DateTime fecha_genera_dato { get; set; }
        public long clave_sala { get; set; }
        public string nombre_cirugia { get; set; }
        public string nombre_sala { get; set; }
        public long clave_cirugia { get; set; }
        public string estado_cirugia { get; set; }
    }
}