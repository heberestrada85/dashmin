/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

namespace Dashmin.Application.Common.Models
{
    public class Indicator
    {
        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public int IdIndicator { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public string StoreProcedure { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public string ExportationDate { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public bool ExportHistoric { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public bool CanExport { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public int DaysMemory { get; set; }

        /// <summary>
        /// Gets or sets a value
        /// </summary>
        public string BusinessName { get; set; }
    }
}