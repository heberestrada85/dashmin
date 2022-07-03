/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Dashmin.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Dashmin.Application.Reports.Commands;

namespace Dashmin.Server.Controllers
{
    /// <summary>
    /// Controlador para las llamadas de los metodos existenes.
    /// </summary>
    public class DataController : ApiController
    {
        /// <summary>
        /// Realiza la obtencion de informacion desde oracle
        /// </summary>
        /// <returns>Result</returns>
        [AllowAnonymous]
        [HttpPost("GetData")]
        public async Task<Result> GetData()
        {
            return await Mediator.Send(new GetData());
        }

        /// <summary>
        /// Realiza la obtencion de informacion desde oracle
        /// </summary>
        /// <returns>Result</returns>
        //[AllowAnonymous]
        //[HttpGet("UpdateRequest")]
        //public async Task<Result> UpdateRequest(string businessName)
        //{
        //    return await Mediator.Send(new UpdateRequest(businessName));
        //}

        /// <summary>
        /// Realiza la actualizacion de informacion en postgres
        /// </summary>
        /// <param name="model">el usuario que queremos autenticar</param>
        /// <returns>Result</returns>
        [AllowAnonymous]
        [HttpPost("UpdateDashboard")]
        [DisableRequestSizeLimit] 
        public async Task<Result> UpdateDashboard(List<IndicatorResult> model)
        {
            return await Mediator.Send(new UpdateDashboard(model));
        }

        /// <summary>
        /// Realiza la actualizacion de informacion en postgres
        /// </summary>
        /// <param name="model">el usuario que queremos autenticar</param>
        /// <returns>Result</returns>
        [AllowAnonymous]
        [HttpPost("DeleteInfoDashboard")]
        [DisableRequestSizeLimit] 
        public async Task<Result> RemoveInformation(Indicator model)
        {
            return await Mediator.Send(new DeleteDashboard(model));
        }
    }
}
