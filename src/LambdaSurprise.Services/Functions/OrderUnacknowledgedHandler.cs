using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using LambdaSurprise.Services.Contracts.Messages;
using LambdaSurprise.Services.OptimizelySdk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LambdaSurprise.Services.Functions
{
    public class OrderUnacknowledgedHandler
    {
        private const string ExperimentKey = "OrderAssistanceExperiment";
        private readonly ILogger<OrderUnacknowledgedHandler> _logger;
        private readonly ITwilioWrapper _twilioWrapper;
        private IOptimizelyWrapper _optimizelyWrapper;

        /// <summary>
        /// IMPORTANT : Default constructor is used in Lambda
        /// 
        /// </summary>
        public OrderUnacknowledgedHandler()
        {
            var serviceProvider = Bootstrap();
            _logger = serviceProvider.GetService<ILogger<OrderUnacknowledgedHandler>>();
            _logger.LogInformation("Initialising lambda"); 
            
            _optimizelyWrapper = serviceProvider.GetService<IOptimizelyWrapper>();
            _twilioWrapper = serviceProvider.GetService<ITwilioWrapper>();
        }
        
        public async Task<bool> HandleAsync(SNSEvent @event, ILambdaContext lambdaContext)
        {
            using (_logger.BeginScope($"{lambdaContext.AwsRequestId}"))
            {
                _logger.LogInformation("Beginning execution");
                var message = ParseMessage(@event); 
                
                var theCustomerNeedsToBeCalled = DoesTheCustomerNeedToBeCalled(message);
     
                if (theCustomerNeedsToBeCalled)
                {
                    await CallTheCustomer(message.CustomerTelephoneNumber); 
                }

                _logger.LogInformation("Execution complete");
            }
            return await Task.FromResult(true);
        }

        private static OrderUnacknowledged ParseMessage(SNSEvent @event)
        {
            if (@event != null && @event.Records.Any())
            {
                var sns = @event.Records[0].Sns;
                return JsonConvert.DeserializeObject<OrderUnacknowledged>(sns.Message);
            }

            return new OrderUnacknowledged {CustomerTelephoneNumber = "237462745", OrderId = Guid.NewGuid()};
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
            _logger.LogInformation("Retrieving experiment variant");
            
            var variant = _optimizelyWrapper.GetExperimentVariant(ExperimentKey, message.OrderId);
            
            _logger.LogInformation($"Variant {variant} returned for {message.OrderId}");
            
            return variant == Variants.DoNotCall;
        }
        
        private bool DoesCustomerNeedToIntervene(OrderUnacknowledged message)
        {
            //Perform some checks on the order, and subject to state, then customer needs to act
            //Defaulted to true for purpose of demonstrating threading problem
            
            //in an effort to simulate external api call I've put in a thread sleep
            _logger.LogInformation("Started order state check");
            
            Thread.Sleep(500);
            
            _logger.LogInformation("Completed order state check. Order state is unacknowledged");
            return true;
        }

        private ServiceProvider Bootstrap()
        {
            var builder = new ConfigurationBuilder();
            
            var configuration = builder.Build();
            
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped((provider) => configuration);
            
            serviceCollection.AddLogging(SetupLogger);
            serviceCollection.AddScoped<ITwilioWrapper, FakeTwilioWrapper>();
            serviceCollection.AddScoped<IOptimizelyWrapper, OptimizelyWrapper>();
            serviceCollection.AddScoped<IHttpClientEventDispatcher, HttpClientEventDispatcher>(); 
            
            return serviceCollection.BuildServiceProvider();
        }

        private void SetupLogger(ILoggingBuilder loggingBuilder)
        {
            var loggerOptions = new LambdaLoggerOptions
            {
                IncludeCategory = true,
                IncludeLogLevel = true,
                IncludeNewline = true,
                IncludeEventId = true,
                IncludeException = true,
                IncludeScopes = true
            };
            
            // Configure Lambda logging
            loggingBuilder.AddLambdaLogger(loggerOptions);
            
        }
    }
}