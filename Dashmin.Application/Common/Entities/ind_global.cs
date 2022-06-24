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
    public class ind_global
    {
        public long cuenta { get; set; }
        public DateTime fechaegreso { get; set; }
        public string area { get; set; }
        public string procedimiento { get; set; }
        public string diagnostico { get; set; }
        public long importe { get; set; }
        public long edad { get; set; }
        public string sexo { get; set; }
        public string medico { get; set; }
        public string convenio { get; set; }
        public long diasestancia { get; set; }
        public long infeccion { get; set; }
        public string cp { get; set; }
        public string paiscp { get; set; }
        public string estadocp { get; set; }
        public string ciudadcp { get; set; }
    }
}