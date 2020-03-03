using System;

namespace LambdaSurprise.Services.Contracts.Messages
{
    public class OrderUnacknowledged
    {
        public Guid OrderId { get; set; }
        public string CustomerTelephoneNumber { get; set; }
    }
}