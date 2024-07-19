using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Diagnostics;

namespace Tomate.HttpRequest
{
    public class HttpRequest
    {
        //URL del servidor
        public static string? SERVIDOR_IP = null;
        //Token de acceso al sistema
        public static string? TOKEN_ACCESO = null;

        /**
         * Genera una instancia RestClient con el token y url configurada
         */
        private static RestClient getRestClient()
        {
            /*if (TOKEN_ACCESO == null)
            {
                TOKEN_ACCESO = Configuracion.getValor(Configuracion.Key.TOKEN_ACCESO);
            }
            if (SERVIDOR_IP == null)
            {
                SERVIDOR_IP = $"http://{Configuracion.getValor(Configuracion.Key.DIRECCION_SERVIDOR)}";
            }*/
#pragma warning disable CS8604 // Possible null reference argument.
            RestClient client = new(baseUrl: $"{SERVIDOR_IP}");
#pragma warning restore CS8604 // Possible null reference argument.
            if (TOKEN_ACCESO != null)
            {
                client.AddDefaultHeader("x-api-key", TOKEN_ACCESO);
            }
            //client.Options.MaxTimeout = 3000;
            return client;
        }

        /**
         * Envia la peticion Http por Get
         */
        public static dynamic sendGet(RestRequest request)
        {
            var client = getRestClient();
            var response = client.Get(request);
#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
            return JsonConvert.DeserializeObject($"{response.Content}");
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
        }

        /**
         * Envia la peticion Http por Post
         */
        public static JObject send(RestRequest request)
        {
            string error = "{'error':'No se pudo conectar con el servidor, por favor revisa tu conexión'}";
            try
            {
                var client = getRestClient();
                var response = client.Execute(request);
                int numericStatusCode = (int)response.StatusCode;
                string? jsonEncodeResponse = response.Content;
                if (numericStatusCode == 401)
                {
                    jsonEncodeResponse = "{'error':'La información de configuración es incorrecta'}";
                }
                else if (numericStatusCode == 0 || jsonEncodeResponse == null || jsonEncodeResponse.Length == 0)
                {
                    jsonEncodeResponse = error;
                }

#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
                return JsonConvert.DeserializeObject(jsonEncodeResponse) as JObject;
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
            }
            catch (Exception e)
            {
#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
                return JsonConvert.DeserializeObject(error) as JObject;
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo>
            }

        }

    }
}
