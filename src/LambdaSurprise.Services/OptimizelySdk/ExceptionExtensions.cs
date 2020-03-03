using System;

namespace LambdaSurprise.Services.OptimizelySdk
{
    public static class ExceptionExtensions
    {
        public static string GetAllMessages(this Exception exception, string separator = "\n")
        {
            if (exception.InnerException == null)
                return exception.Message;

            return (string.IsNullOrEmpty(exception.Message) ? "" : string.Format("{0}{1}", exception.Message, separator))
                   + GetAllMessages(exception.InnerException, separator);
        }
    }
}