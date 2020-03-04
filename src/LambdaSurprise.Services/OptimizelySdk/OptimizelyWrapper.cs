using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;

namespace LambdaSurprise.Services.OptimizelySdk
{
    public class OptimizelyWrapper : IOptimizelyWrapper
    {
        private readonly IHttpClientEventDispatcher _httpClientEventDispatcher;
        private readonly ILogger<OptimizelyWrapper> _logger;

        public OptimizelyWrapper(IHttpClientEventDispatcher httpClientEventDispatcher, ILogger<OptimizelyWrapper> logger)
        {
            _httpClientEventDispatcher = httpClientEventDispatcher;
            _logger = logger;
        }
        
        /// <summary>
        /// Imagine this is a call to Optimizely to determine if the expiriment is active
        /// And to determine if the customer is in the experimental group to be called or not
        /// </summary>
        /// <param name="experimentKey"></param>
        /// <param name="messageOrderId"></param>
        /// <returns></returns>
        public string GetExperimentVariant(string experimentKey, object messageOrderId)
        {
            //Record the activity in optimizely
            SendImpressionEvent(); 
            
            return Variants.Call;
        }
        private void SendImpressionEvent()
        {
            //Imagine if you will, that this call takes a couple of seconds
            _httpClientEventDispatcher.DispatchEvent(CreateLogEvent());
        }

        private LogEvent CreateLogEvent()
        { 
            //https://github.com/optimizely/csharp-sdk/blob/b8dc00e72eb58b46d2f4dc94ab9bb611801794a3/OptimizelySDK/Event/EventFactory.cs#L57

            var parameters = new Dictionary<string, object>();
            var headers = new Dictionary<string, string>();
            
            //real url is "https://logx.optimizely.com/v1/events"
            return new LogEvent("http://abc.com/v1/events", parameters, WebRequestMethods.Http.Post, headers);
        }

    }
}