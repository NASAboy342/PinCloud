using APinI.Services;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace APinI.BE
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;
        public HttpClientService()
        {
            _httpClient = new HttpClient();
        }
        public Task<bool> CheckUrlValidity(string url)
        {
            try
            {
                var uri = new Uri(url);
                return Task.FromResult(true); // URL is valid
            }
            catch (UriFormatException)
            {
                return Task.FromResult(false); // URL is invalid
            }
        }

        public async Task<TResponse> Form<TResponse>(string url, Dictionary<string, string> req) where TResponse : class
        {
            var formData = new FormUrlEncodedContent(req);
            HttpResponseMessage response = await _httpClient.PostAsync(url, formData);

            // Check if the response is successful (status code 200-299)
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response content to your desired type (TResponse)
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TResponse>(responseBody);
                return result;
            }
            else
            {
                // Handle non-successful responses (e.g., log, throw an exception, etc.)
                // You might want to create a custom exception type for API-related errors
                // For now, let's return null as a placeholder
                return null;
            }

        }

        public async Task<TResponse> Get<TResponse>(string url) where TResponse : class
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            // Check if the response is successful (status code 200-299)
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response content to your desired type (TResponse)
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TResponse>(responseBody);
                return result;
            }
            else
            {
                // Handle non-successful responses (e.g., log, throw an exception, etc.)
                // You might want to create a custom exception type for API-related errors
                // For now, let's return null as a placeholder
                return null;
            }

        }

        public async Task<TResponse> Post<TResponse, TRequest>(string url, TRequest req) where TResponse : class
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(url, jsonContent);

            // Check if the response is successful (status code 200-299)
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response content to your desired type (TResponse)
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TResponse>(responseBody);
                return result;
            }
            else
            {
                // Handle non-successful responses (e.g., log, throw an exception, etc.)
                // You might want to create a custom exception type for API-related errors
                // For now, let's return null as a placeholder
                return null;
            }

        }

        public async Task<TResponse> Post<TResponse>(string url) where TResponse : class
        {
            HttpResponseMessage response = await _httpClient.PostAsync(url, null);

            // Check if the response is successful (status code 200-299)
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response content to your desired type (TResponse)
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TResponse>(responseBody);
                return result;
            }
            else
            {
                // Handle non-successful responses (e.g., log, throw an exception, etc.)
                // You might want to create a custom exception type for API-related errors
                // For now, let's return null as a placeholder
                return null;
            }
        }
        public async Task<string> Post(string url)
        {
            HttpResponseMessage response = await _httpClient.PostAsync(url, null);

            // Check if the response is successful (status code 200-299)
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response content to your desired type (TResponse)
                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            else
            {
                // Handle non-successful responses (e.g., log, throw an exception, etc.)
                // You might want to create a custom exception type for API-related errors
                // For now, let's return null as a placeholder
                return null;
            }
        }
    }
}