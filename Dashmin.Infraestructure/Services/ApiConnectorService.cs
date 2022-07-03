/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dashmin.Application.Common.Interface;
using Dashmin.Application.Common.Models;
using System.Collections.Generic;

namespace Dashmin.Infraestructure.Services
{
    /// <summary>
    /// Servicio para obtener informacion mediante APIRestFull, externa.
    /// </summary>
    public class ApiConnectorService : IApiConnectorService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly JsonSerializerOptions _options;
        private string _httpMethod = "GET";
        private string _ruta;
        private StringContent _contentToSend;

        private HttpRequestMessage request;

        /// <summary>
        /// Constructor de la clase ApiConnectorService
        /// </summary>
        public ApiConnectorService( IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = false,
            };
        }

        /// <summary>
        /// Selector de HttpMethod, POST, GET, PUT, DELETE
        /// </summary>
        /// <param name="httpMethod"> Metodo HttpMethod para procesar la petición </param>
        public void HttpMethodSelector(string httpMethod)
        {
            _httpMethod = httpMethod;
        }

        /// <summary>
        /// Metodo de obtencion de datos de un API RestFull, requiere inicializar con el tipo o entidad del valor a obtener
        /// </summary>
        /// <param name="apiAddress"> EndPoint de la peticion que se realiza </param>
         public async Task<(Result,T)> GetDataFromApi<T>(string apiAddress)
        {
            _ruta = $"{apiAddress}";
            return await this.SendDataToApi<T>();
        }

        /// <summary>
        /// Metodo de obtencion de datos de un API RestFull, requiere inicializar con el tipo o entidad del valor a obtener
        /// </summary>
        /// <param name="apiAddress"> EndPoint de la peticion que se realiza </param>
        /// <param name="dataToSend"> Parametros a enviar en la solicitud del api service </param>
        public async Task<(Result,T)> GetDataFromApi<T,T2>(string apiAddress,T2 dataToSend)
        {
            _ruta = $"{apiAddress}";
            _contentToSend = new StringContent(JsonConvert.SerializeObject(dataToSend), Encoding.UTF8, "application/json");
            return await this.SendDataToApi<T>();
        }

        public async Task<(Result,T)> SendDataToApi<T>()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            switch (_httpMethod)
            {
                case "GET":
                    request = new HttpRequestMessage(HttpMethod.Get, _ruta);
                    break;
                case "POST":
                    request = new HttpRequestMessage(HttpMethod.Post, _ruta);
                    break;
                case "PUT":
                    request = new HttpRequestMessage(HttpMethod.Put, _ruta);
                    break;
                case "DELETE":
                    request = new HttpRequestMessage(HttpMethod.Delete, _ruta);
                    break;
            }

            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "Dashmin-ApiConnector");
            if ( _contentToSend != null ) 
                request.Content = _contentToSend;

            HttpClient client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                IEnumerable<string> errors = new List<string>() { response.ReasonPhrase };
                return (Result.Failure(errors), default);
            }

            string content = await response.Content.ReadAsStringAsync();
            T data = JsonConvert.DeserializeObject<T>(content);
            return (Result.Success(), data);
        }
    }
}
