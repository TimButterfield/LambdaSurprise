using System.Threading.Tasks;

namespace LambdaSurprise.Services.Functions
{
    public interface ITwilioWrapper
    {
        Task CallAsync(string telephoneNumber); 
    }
}