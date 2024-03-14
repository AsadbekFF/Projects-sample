using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Common.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetInnerExceptionMessage(this Exception exception)
        {
            if (exception is AggregateException aggregateException)
            {
                var stringBuilder = new StringBuilder();

                foreach (var innerException in aggregateException.InnerExceptions)
                    BuildAggregateErrorMessage(innerException, stringBuilder);

                return stringBuilder.ToString();
            }

            return GetInnerMessage(exception);
        }

        private static string GetInnerMessage(Exception exception)
        {
            string message = null;

            while (exception != null)
            {
                message = exception.Message;
                exception = exception.InnerException;
            }

            return message;
        }

        private static void BuildAggregateErrorMessage(Exception exception, StringBuilder stringBuilder)
        {
            if (exception is AggregateException aggregateException)
                foreach (var innerException in aggregateException.InnerExceptions)
                    BuildAggregateErrorMessage(innerException, stringBuilder);
            else
                stringBuilder.AppendLine(GetInnerMessage(exception));
        }
    }
}
