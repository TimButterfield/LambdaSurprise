using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LambdaSurprise.Services.Functions
{
    public class FakeTwilioWrapper : ITwilioWrapper
    {
        private readonly ILogger<FakeTwilioWrapper> _logger;

        public FakeTwilioWrapper(ILogger<FakeTwilioWrapper> logger)
        {
            _logger = logger;
        }
        
        public async Task CallAsync(string telephoneNumber)
        {
            //NoOp : 
            _logger.LogInformation("Queuing call to customer");
            
            Thread.Sleep(300);
            
            _logger.LogInformation("Call successfully queued to customer");
            
            await Task.CompletedTask;
        }
    }
}