/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System.Linq;
using System.Collections.Generic;

namespace Dashmin.Application.Common.Models
{
    /// <summary>
    /// Manejando los result en una sola clase. de si fue exitoso o no
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="succeeded">if set to <c>true</c> [succeeded].</param>
        /// <param name="errors">The errors.</param>
        internal Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Result"/> is succeeded.
        /// </summary>
        /// <value><c>true</c> if succeeded; otherwise, <c>false</c>.</value>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        /// <value>The errors.</value>
        public string[] Errors { get; set; }

        /// <summary>
        /// Successes this instance.
        /// </summary>
        /// <returns>Result.</returns>
        public static Result Success()
        {
            return new Result(true, new string[] { });
        }

        /// <summary>
        /// Failures the specified errors.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <returns>Result.</returns>
        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }
    }
}
