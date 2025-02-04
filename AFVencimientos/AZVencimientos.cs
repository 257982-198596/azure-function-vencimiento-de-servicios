using System;
using System.Text.Json;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AFVencimientos
{
    public class AZVencimientos
    {
        private readonly HttpClient _httpClient;

        public AZVencimientos(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [Function("PostToApi")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, FunctionContext context)
        {
            //"0 0 0 * * *"
            var logger = context.GetLogger("PostToApi");
            logger.LogInformation($"Funcion ejecutada: {DateTime.UtcNow}");


            string baseUrl = Environment.GetEnvironmentVariable("VITE_REACT_APP_API_URL");
            string apiUrl = $"{baseUrl}serviciosdelcliente/cambiar-estado-servicios-del-cliente-a-vencidos";
            string apiKey = Environment.GetEnvironmentVariable("VITE_REACT_APP_API_KEY");


            var requestData = new
            {

            };

            var requestJson = JsonSerializer.Serialize(requestData);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            // API Key a los headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

            try
            {
                //post
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation($"Éxito: {responseContent}");
                }
                else
                {
                    logger.LogError($"Error: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Excepción: {ex.Message}");
            }
        }
    }
}
