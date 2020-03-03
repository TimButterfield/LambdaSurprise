using Amazon.Lambda.Core;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace LambdaSurprise.Services
{
    public class AssemblyInfo
    {
        
    }
}