using System.Collections.Generic;

namespace LambdaSurprise.Services.OptimizelySdk
{
    public class LogEvent
    {
        /// <summary>
        /// string URL to dispatch log event to
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Parameters to be set in the log event
        /// </summary>
        public Dictionary<string, object> Params { get; private set; }

        /// <summary>
        /// HTTP verb to be used when dispatching the log event
        /// </summary>
        public string HttpVerb { get; private set; }

        /// <summary>
        /// Headers to be set when sending the request
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; }

        public string GetParamsAsJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(Params);
        }

        /// <summary>
        /// LogEvent Construtor
        /// </summary>
        public LogEvent(string url, Dictionary<string, object> parameters, string httpVerb, Dictionary<string, string> headers)
        {
            Url = url;
            Params = parameters;
            HttpVerb = httpVerb;
            Headers = headers;
        }
    }
}