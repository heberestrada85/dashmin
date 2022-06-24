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
    public class var_cuenta_paciente
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public DateTime fecha_dato { get; set; }
        public long empresa_contable { get; set; }
        public long organizacion_id { get; set; }
        public long clave_cuenta_paciente { get; set; }
        public string nombre_paciente { get; set; }
        public string sexo_paciente { get; set; }
        public long edad_paciente { get; set; }
        public long clave_medico_responsable { get; set; }
        public string nombre_medico_responsable { get; set; }
        public long clave_especialidad_medico { get; set; }
        public string nombre_especialidad_medico { get; set; }
        public DateTime fecha_genera_dato { get; set; }
        public string codigo_postal { get; set; }
        public string tipo_ingreso { get; set; }
        public long clave_departamento { get; set; }
        public string nombre_departamento { get; set; }
        public string nombre_estado { get; set; }
        public string nombre_ciudad { get; set; }
        public string nombre_pais { get; set; }
    }
}