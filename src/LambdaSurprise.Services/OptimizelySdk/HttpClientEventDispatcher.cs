using System;
using System.Net.Http;
using System.Threading.Tasks;
using LambdaSurprise.Services.Functions;

namespace LambdaSurprise.Services.OptimizelySdk
{
    public class HttpClientEventDispatcher
    {
        public ILogger Logger { get; set; } = new DefaultLogger();
        
        /// <summary>
        /// HTTP client object.
        /// </summary>
        private static readonly HttpClient Client;

        /// <summary>
        /// Constructor for initializing static members.
        /// </summary>
        static HttpClientEventDispatcher()
        {
            Client = new HttpClient();
        }

        public void DispatchEvent(LogEvent logEvent)
        {
            Task.Run(() => DispatchEventAsync(logEvent));
        }
    
        private async void DispatchEventAsync(LogEvent logEvent)
        {
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

                
                //var result = await Client.SendAsync(request);
                //result.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.ERROR, string.Format("Error Dispatching Event: {0}", ex.GetAllMessages()));
            }
        }
    }
}