/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System.Threading.Tasks;
using Dashmin.Application.Common.Models;

namespace Dashmin.Application.Common.Interface
{
    public interface IApiConnectorService
    {
        public Task<(Result,T)> GetDataFromApi<T>( string apiAddress);
        public Task<(Result,T)> GetDataFromApi<T>( string apiAddress,T dataToSend);
        public void HttpMethodSelector( string httpMethod );
    }
}
