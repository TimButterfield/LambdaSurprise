using System.Threading.Tasks;
using LambdaSurprise.Services.OptimizelySdk;

namespace LambdaSurprise.Services.Functions
{
    public class FakeTwilioWrapper : ITwilioWrapper
    {
        private readonly ILogger _logger;

        public FakeTwilioWrapper(ILogger logger)
        {
            _logger = logger;
        }
        
        public async Task CallAsync(string telephoneNumber)
        {
            //NoOp : 
            await Task.CompletedTask;
        }
    }
}