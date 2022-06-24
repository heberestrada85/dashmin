/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dashmin.Application.Common.Exceptions
{
    /// <summary>
    /// Class ValidationDataAnnotations.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ValidationDataAnnotations
    {
        
        private ValidationContext _validationContext;
        private List<ValidationResult> _resultsValidation;
        private object _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationDataAnnotations"/> class.
        /// </summary>
        /// <param name="instance">model/instance to validate.</param>
        public ValidationDataAnnotations(object instance){

            _validationContext = new ValidationContext(instance, null, null);
            _resultsValidation = new List<ValidationResult>();
            _instance = instance;

        }

        /// <summary>
        /// Verify the model is valid.
        /// </summary>
        public bool IsValid(){
            return Validator.TryValidateObject(_instance, _validationContext, _resultsValidation, true);        
        }

        /// <summary>
        /// Get Errors.
        /// </summary>
        public List<string> Errors(){
            return _resultsValidation.Select(v => v.ErrorMessage).ToList();      
        }
    
    }
}