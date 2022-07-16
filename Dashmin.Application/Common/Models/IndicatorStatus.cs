/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System;

namespace Dashmin.Application.Common.Models
{
    public class IndicatorStatus
    {
        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public long Business { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public string BusinessShortName { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public string BusinessName { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public string IdIndicator { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public string Indicator { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}