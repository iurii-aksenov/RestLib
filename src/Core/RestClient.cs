using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.Authentication;
using Core.Infrastructure;
using Newtonsoft.Json;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace Core
{
    public abstract class RestClient
    {
        private readonly HttpClient _httpClient;
        private readonly RestOptions _restSettings;

        protected RestClient(string serviceName, HttpClient httpClient, RestOptions restSettings)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _restSettings = restSettings;

            if (restSettings is null)
            {
                throw new ArgumentNullException(nameof(restSettings));
            }

            _httpClient.BaseAddress = restSettings.GetUri(serviceName);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "OCS-Rest-Api");
        }

        protected Task<RestResponse<TResponseData>> CallAsync<TResponseData>(string url)
        {
            return CallAsync<TResponseData, object>(url, new object());
        }

        protected async Task<RestResponse<TResponseData>> CallAsync<TResponseData, TRequestData>(string url,
            TRequestData request)
        {
            _httpClient.DefaultRequestHeaders.Remove("Rest-Auth");
            var token = RestTokenBuilder.CreateToken(_restSettings.RestToken);
            _httpClient.DefaultRequestHeaders.Add("Rest-Auth", token);
            try
            {
                var json = JsonConvert.SerializeObject(request);
                using HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RestResponse<TResponseData>>(responseContent);
            }
            catch (HttpRequestException ex)
            {
                var error = new RestError((int) RestErrorType.ConnectionError, ex.Message,
                    nameof(RestErrorType.ConnectionError), ex.StackTrace);
                return new RestResponse<TResponseData>(error);
            }
            catch (TimeoutRejectedException rejectedException)
            {
                var error = new RestError((int) RestErrorType.ConnectionError, rejectedException.Message,
                    nameof(RestErrorType.ConnectionError), rejectedException.StackTrace);
                return new RestResponse<TResponseData>(error);
            }
            catch (BrokenCircuitException brokenCircuitException)
            {
                var error = new RestError((int) RestErrorType.ConnectionError, brokenCircuitException.Message,
                    nameof(RestErrorType.ConnectionError), brokenCircuitException.StackTrace);
                return new RestResponse<TResponseData>(error);
            }
        }
    }
}