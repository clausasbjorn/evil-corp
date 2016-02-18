using System;
using System.Net.Http;
using EvilCorp.SignalHub.Properties;

namespace EvilCorp.SignalHub.GoogleApi
{
    public class UrlShortener
    {
        private const string ApiBaseUrl = "https://www.googleapis.com/urlshortener/v1/url";
        private const string RequestFormatUrl = "url?key={0}";

        public string GetQrCodeForUrl(string url)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ApiBaseUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();

                var result = httpClient.PostAsJsonAsync(string.Format("?key={0}", Settings.Default.ApiKey), new Request { LongUrl = url }).Result;

                if (result.IsSuccessStatusCode)
                {
                    var response = result.Content.ReadAsAsync<Response>().Result;
                    return response.Id + ".qr";
                }
                else
                {
                    var error = result.Content.ReadAsAsync<ErrorResponse>().Result;
                    throw new Exception(error.Error.Message);
                }

                throw new Exception("Could not get qr code for url");
            }
        }
    }
}