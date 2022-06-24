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
    public class ind_pacientes_egresados
    {
        public long numerocuenta { get; set; }
        public DateTime fechaegreso { get; set; }
        public DateTime fechanacimiento { get; set; }
        public string sexobp { get; set; }
        public long edad { get; set; }
        public string grupoetario { get; set; }
        public string area { get; set; }
        public string cie10 { get; set; }
        public string descripciondiagnostico { get; set; }
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public long organizacionid { get; set; }
        public long empresacontable { get; set; }
        public DateTime fechadato { get; set; }
        public long grupoetarioid { get; set; }
        public long areaid { get; set; }
    }
}