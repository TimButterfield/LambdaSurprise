using System.Diagnostics;

namespace LambdaSurprise.Services.OptimizelySdk
{
    public class DefaultLogger : ILogger
    {
        public void Log(LogLevel level, string message)
        {
            string line = string.Format("[{0}] : {1}", level, message);
            Debug.WriteLine(line);
        }
    }
}