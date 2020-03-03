using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using LambdaSurprise.Services.Contracts.Messages;
using LambdaSurprise.Services.OptimizelySdk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ILogger = LambdaSurprise.Services.OptimizelySdk.ILogger;

namespace LambdaSurprise.Services.Functions
{
    public class OrderUnacknowledgedHandler
    {
        private const string ExperimentKey = "OrderAssistanceExperiment";
        private readonly ILogger _logger;
        private readonly ITwilioWrapper _twilioWrapper;
        private IOptimizelyWrapper _optimizelyWrapper;

        /// <summary>
        /// IMPORTANT : Default constructor is used in Lambda
        /// 
        /// </summary>
        public OrderUnacknowledgedHandler()
        {
            var serviceProvider = Bootstrap();
            _logger = serviceProvider.GetService<ILogger>();
            _optimizelyWrapper = serviceProvider.GetService<IOptimizelyWrapper>();
            _twilioWrapper = serviceProvider.GetService<ITwilioWrapper>();

        }
        
        public async Task<bool> HandleAsync(SNSEvent @event, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.Log("Beginning execution");
            var sns = @event.Records[0].Sns; 
            var message = JsonConvert.DeserializeObject<OrderUnacknowledged>(sns.Message); 
            var theCustomerNeedsToBeCalled = DoesTheCustomerNeedToBeCalled(message);
 
            if (theCustomerNeedsToBeCalled)
            {
                await CallTheCustomer(message.CustomerTelephoneNumber); 
            }

            lambdaContext.Logger.Log("Execution complete");
            return await Task.FromResult(true);
        }

        private async Task CallTheCustomer(string customerTelephoneNumber)
        {
            await _twilioWrapper.CallAsync(customerTelephoneNumber); 
        }
        

        private bool DoesTheCustomerNeedToBeCalled(OrderUnacknowledged message)
        { 
            var doesTheOrderRequiredCustomerIntervention = DoesCustomerNeedToIntervene(message);
            var isTheCustomerInTheCallGroup = IsTheCustomerInTheControlGroup(message);

            return doesTheOrderRequiredCustomerIntervention && !isTheCustomerInTheCallGroup;
        }

        private bool IsTheCustomerInTheControlGroup(OrderUnacknowledged message)
        {
            var variant = _optimizelyWrapper.GetExperimentVariant(ExperimentKey, message.OrderId);
            return variant == Variants.DoNotCall;
        }
        
        private bool DoesCustomerNeedToIntervene(OrderUnacknowledged message)
        {
            //Perform some checks on the order, and subject to state, then customer needs to act
            //Defaulted to true for purpose of demonstrating threading problem
            return true;
        }

        private ServiceProvider Bootstrap()
        {
            var builder = new ConfigurationBuilder();
            var configuration = builder.Build();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped((provider) => configuration);
            serviceCollection.AddLogging(SetupLogger);
            serviceCollection.AddScoped<ILogger, DefaultLogger>(); 
            serviceCollection.AddScoped<ITwilioWrapper, FakeTwilioWrapper>();
            serviceCollection.AddScoped<IOptimizelyWrapper, OptimizelyWrapper>(); 
            
            return serviceCollection.BuildServiceProvider();
        }

        private void SetupLogger(ILoggingBuilder logging)
        {
            var loggerOptions = new LambdaLoggerOptions
            {
                IncludeCategory = true,
                IncludeLogLevel = true,
                IncludeNewline = true,
                IncludeEventId = true,
                IncludeException = true
            };

            // Configure Lambda logging
            logging.AddLambdaLogger(loggerOptions);
        }
    }
}