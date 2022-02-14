using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    public class SimConnectGpsLocationData : ILocationData
    {
        HttpClientHandler _clientHandler = new HttpClientHandler();
        JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public SimConnectGpsLocationData()
        {
            _clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        }

        public async Task<Location> Get(GeolocationAccuracy accuracy, int timeout, CancellationToken ct)
        {
            Location location;

            using (var httpClient = new HttpClient())
            {
                try
                {
                    using (var response = await httpClient.GetAsync(Preferences.Get("SimConnectApiUrl", string.Empty)))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();
                            location = JsonSerializer.Deserialize<Location>(content, _jsonSerializerOptions);
                        }
                        else
                        {
                            location = null;
                        }
                    }
                }
                catch
                {
                    location = null;
                }
            }

            return location;
        }
    }
}