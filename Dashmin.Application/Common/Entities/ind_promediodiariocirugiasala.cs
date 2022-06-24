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
    public class ind_promediodiariocirugiasala
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public long organizacionid { get; set; }
        public long empresacontable { get; set; }
        public DateTime fecha_dato { get; set; }
        public long clavesala { get; set; }
        public string descripcionsala { get; set; }
        public long totalcirugiassala { get; set; }
        public long diastranscurridos { get; set; }
    }
}