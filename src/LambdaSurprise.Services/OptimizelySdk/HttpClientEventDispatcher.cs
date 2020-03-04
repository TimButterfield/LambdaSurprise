using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LambdaSurprise.Services.OptimizelySdk
{
    public class HttpClientEventDispatcher : IHttpClientEventDispatcher
    {
        private readonly ILogger<HttpClientEventDispatcher> _logger;
        /// <summary>
        /// HTTP client object.
        /// </summary>
        //private static readonly HttpClient Client;

        /// <summary>
        /// Constructor for initializing static members.
        /// </summary>
        public HttpClientEventDispatcher(ILogger<HttpClientEventDispatcher> logger)
        {
            _logger = logger;
            //Client = new HttpClient();
        }
     

        public void DispatchEvent(LogEvent logEvent)
        {
            Task.Run(() => DispatchEventAsync(logEvent));
        }
    
        private async void DispatchEventAsync(LogEvent logEvent)
        {
            _logger.LogInformation("Logging event activity");
            try
            {
                var json = logEvent.GetParamsAsJson();
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(logEvent.Url),
                    Method = HttpMethod.Post,
                    // The Content-Type header applies to the Content, not the Request itself
                    Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
                };

                foreach (var header in logEvent.Headers)
                    if (header.Key.ToLower() != "content-type")
                        request.Content.Headers.Add(header.Key, header.Value);

                //Note the real code makes a HTTP request to an api to record an event
                Thread.Sleep(500);
                _logger.LogInformation("Finished logging event activity");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error Dispatching Event: {ex.GetAllMessages()}");
            }
        }
    }

    public interface IHttpClientEventDispatcher
    {
        void DispatchEvent(LogEvent logEvent);
    }
}